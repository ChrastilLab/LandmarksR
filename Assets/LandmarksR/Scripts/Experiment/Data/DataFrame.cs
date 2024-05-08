#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace LandmarksR.Scripts.Experiment.Data
{
    public enum MergeType
    {
        Horizontal,
        Vertical
    }
    public class DataFrame
    {
        // Fields
        private readonly ColumnNameMap _columnNameMap;
        private readonly List<List<object?>> _data = new();

        // Constructors
        public DataFrame()
        {
            ColumnCount = 0;
            _columnNameMap = new ColumnNameMap();
        }

        public DataFrame(DataFrame dataFrame)
        {
            ColumnCount = dataFrame.ColumnCount;
            _columnNameMap = new ColumnNameMap(dataFrame._columnNameMap);
            foreach (var row in dataFrame._data)
            {
                _data.Add(new List<object?>(row));
            }
        }

        public DataFrame(IReadOnlyList<string> columnNames)
        {
            ColumnCount = columnNames.Count;
            _columnNameMap = new ColumnNameMap();
            SetColumnNames(columnNames);
        }


        // Properties
        public IReadOnlyList<List<object?>> Rows => _data;
        public int RowCount => _data.Count;
        public int ColumnCount { get; private set; }


        // Public Methods

        #region Public Methods

        /// <summary>
        /// Append a row to the DataFrame.
        /// </summary>
        /// <param name="singleRow">The elements in a row</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AppendRow(List<object?> singleRow)
        {
            if (singleRow == null) throw new ArgumentNullException(nameof(singleRow));

            // Ensure same number of row and col
            if (RowCount > 0 && singleRow.Count != ColumnCount)
                throw new ArgumentException(
                    $"Row must have the same number of columns as the DataFrame. adding:{singleRow.Count} != current:{ColumnCount}");

            // Update the column count if the row is the first one
            if (RowCount == 0)
            {
                UpdateColumnCount(singleRow.Count);
                SetPlaceholderColumnNames();
            }

            _data.Add(singleRow);
        }


        /// <summary>
        ///  Append a column to the DataFrame.
        /// </summary>
        /// <param name="singleColumn">The elements in a column</param>
        /// <param name="columnName">The column's name</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void AppendColumn(List<object?> singleColumn, string columnName)
        {
            if (singleColumn == null) throw new ArgumentNullException(nameof(singleColumn), "Column cannot be null.");
            if (columnName == null) throw new ArgumentNullException(nameof(columnName), "Column name cannot be null.");

            // Ensure same number of row and col
            if (RowCount > 0 && singleColumn.Count != RowCount)
                throw new ArgumentException("Column must have the same number of rows as the DataFrame.");

            // Update the column count if the column is the first one
            if (RowCount == 0)
            {
                UpdateColumnCount(1);

                SetColumnName(0, columnName);

                for (var i = 0; i < singleColumn.Count; i++)
                {
                    _data.Add(new List<object?> { singleColumn[i] });
                }
            }
            else
            {
                UpdateColumnCount(1 + ColumnCount);
                SetColumnName(ColumnCount - 1, columnName);

                // Add the column to the DataFrame
                for (var i = 0; i < singleColumn.Count; i++)
                {
                    _data[i].Add(singleColumn[i]);
                }
            }
        }


        /// <summary>
        /// Set the column names of the DataFrame.
        /// </summary>
        /// <param name="columnNames">A List containing the column names</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetColumnNames(IReadOnlyList<string> columnNames)
        {
            if (columnNames.Count > ColumnCount)
                throw new ArgumentException(
                    $"Headers must have the same as or less than the length of DataFrame columns.\n " +
                    $"{columnNames.Count} > {ColumnCount}");

            _columnNameMap.RemoveAll();
            for (var i = 0; i < columnNames.Count; i++)
            {
                var columnName = GetUniqueColumnName(columnNames[i]);
                _columnNameMap.TryAdd(columnName, i);
            }

            for (var i = columnNames.Count; i < ColumnCount; i++)
            {
                _columnNameMap.TryAdd($"{GetUniqueColumnName("X")}", i);
            }
        }

        private void SetColumnName(int index, string header)
        {
            if (index < 0 || index >= ColumnCount)
                throw new IndexOutOfRangeException("Column index out of range.");


            _columnNameMap.TryRemoveByIndex(index);
            var columnName = GetUniqueColumnName(header);
            _columnNameMap.TryAdd(columnName, index);
        }

        /// <summary>
        ///  Get the value of a specific cell by its row index and column index.
        /// </summary>
        /// <param name="row">0-based index of row</param>
        /// <param name="col">0-based index of column</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T? GetValue<T>(int row, int col)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new IndexOutOfRangeException("Row index out of range.");
            }

            if (col < 0 || col >= ColumnCount)
            {
                throw new IndexOutOfRangeException("Column index out of range.");
            }

            AdjustEmptyRow(row);

            return (T?)_data[row][col];
        }


        /// <summary>
        /// Get the value of a specific cell by its row index and column name.
        /// </summary>
        /// <param name="row">0-based index of row</param>
        /// <param name="columnName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public T? GetValue<T>(int row, string columnName)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new IndexOutOfRangeException("Row index out of range.");
            }

            if (!_columnNameMap.TryGetIndex(columnName, out var col))
            {
                throw new ArgumentException("Header not found.");
            }


            AdjustEmptyRow(row);

            return (T?)_data[row][col];
        }


        /// <summary>
        /// Get the first element of a specific column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <typeparam name="T">The element's type</typeparam>
        /// <returns>The element of specific type</returns>
        /// <exception cref="ArgumentException"></exception>
        public T? GetFirstInColumn<T>(string columnName)
        {
            if (RowCount == 0)
                throw new ArgumentException("DataFrame is empty.");

            if (!_columnNameMap.TryGetIndex(columnName, out var col))
            {
                throw new ArgumentException("Header not found.");
            }

            return GetValue<T>(0, col);
        }

        public void SetValue(int row, int col, object? value)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new IndexOutOfRangeException("Row index out of range.");
            }

            if (col < 0 || col >= ColumnCount)
            {
                throw new IndexOutOfRangeException("Column index out of range.");
            }

            AdjustEmptyRow(row);

            _data[row][col] = value;
        }

        public void SetValue(int row, string columnName, object? value)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new IndexOutOfRangeException("Row index out of range.");
            }

            if (!_columnNameMap.TryGetIndex(columnName, out var col))
            {
                throw new ArgumentException("Header not found.");
            }

            AdjustEmptyRow(row);


            _data[row][col] = value;
        }

        /// <summary>
        /// Get a row by its row index.
        /// </summary>
        /// <param name="row">0-based index of row</param>
        /// <returns>A new DataFrame consisting of that row</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public DataFrame GetRow(int row)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new IndexOutOfRangeException("Row index out of range.");
            }

            var dataFrame = new DataFrame();
            dataFrame.AppendRow(_data[row]);

            dataFrame.SetColumnNames(_columnNameMap.GetOrderedNames());
            return dataFrame;
        }

        /// <summary>
        /// Get a column by its column index.
        /// </summary>
        /// <param name="col">0-based index of column</param>
        /// <returns>A new DataFrame consisting of that column </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public DataFrame GetColumn(int col)
        {
            if (RowCount == 0)
                throw new ArgumentException("DataFrame is empty.");

            if (col < 0 || col >= ColumnCount)
                throw new IndexOutOfRangeException("Column index out of range.");

            var dataFrame = new DataFrame();
            for (var i = 0; i < RowCount; i++)
            {
                AdjustEmptyRow(i);
                dataFrame.AppendRow(new List<object?> { _data[i][col] });
            }

            dataFrame.SetColumnName(0, _columnNameMap.TryGetName(col, out var name) ? name : "X");

            return dataFrame;
        }

        /// <summary>
        ///  Get a column by its column name.
        /// </summary>
        /// <param name="columnName">Column Name</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public DataFrame GetColumn(string columnName)
        {
            if (!_columnNameMap.TryGetIndex(columnName, out var col))
            {
                throw new ArgumentException("Header not found.");
            }

            return GetColumn(col);
        }

        public List<object?> GetRawRow(int row)
        {
            AdjustEmptyRow(row);
            return _data[row];
        }


        /// <summary>
        ///   Get a range of columns from the DataFrame.
        /// </summary>
        /// <param name="startCol">0-based index of column</param>
        /// <param name="count">The number of element you want</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public DataFrame GetColumnRange(int startCol, int count)
        {
            if (startCol < 0 || startCol >= ColumnCount)
            {
                throw new IndexOutOfRangeException("Start index out of range.");
            }

            var end = startCol + count - 1;

            if (end < 0 || end >= ColumnCount)
            {
                throw new IndexOutOfRangeException("End index out of range.");
            }

            if (startCol > end)
            {
                throw new ArgumentException("Start index must be less than or equal to end index.");
            }

            var dataFrame = new DataFrame();
            for (var i = 0; i < RowCount; i++)
            {
                AdjustEmptyRow(i);
                var row = new List<object?>();
                for (var j = startCol; j <= end; j++)
                {
                    row.Add(_data[i][j]);
                }

                dataFrame.AppendRow(row);
            }

            var columnNames = new List<string>();
            for (var i = startCol; i <= end; i++)
            {
                _columnNameMap.TryGetName(i, out var name);
                columnNames.Add(name);
            }

            dataFrame.SetColumnNames(columnNames);
            return dataFrame;
        }

        /// <summary>
        /// Merge two DataFrames by columns.
        /// </summary>
        /// <param name="dataFrame">The Dataframe on the right</param>
        /// <returns>A new Dataframe consisting of merged dataframe</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private DataFrame MergeColumns(DataFrame dataFrame)
        {
            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));

            if (dataFrame.RowCount == 0) return new DataFrame(this);

            if (dataFrame.RowCount != 0 && RowCount != 0 && dataFrame.RowCount != RowCount)
                throw new ArgumentException("DataFrames must have the same number of rows.");

            var newDataFrame = new DataFrame(this);

            if (newDataFrame.RowCount == 0)
            {
                newDataFrame.AddEmptyRows(dataFrame.RowCount);
            }

            for (var i = 0; i < newDataFrame.RowCount; i++)
            {
                newDataFrame._data[i].AddRange(dataFrame._data[i]);
            }

            newDataFrame.AppendColumnNames(dataFrame._columnNameMap.GetOrderedNames());
            newDataFrame.UpdateColumnCount(ColumnCount + dataFrame.ColumnCount);
            return newDataFrame;
        }

        private DataFrame MergeRows(DataFrame dataFrame)
        {
            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));

            if (dataFrame.RowCount == 0) return new DataFrame(this);

            if (dataFrame.ColumnCount != 0 && ColumnCount != 0 && dataFrame.ColumnCount != ColumnCount)
                throw new ArgumentException("DataFrames must have the same number of columns.");

            var newDataFrame = new DataFrame(this);

            if (newDataFrame.ColumnCount == 0)
            {
                newDataFrame.UpdateColumnCount(dataFrame.ColumnCount);
                newDataFrame.SetColumnNames(dataFrame._columnNameMap.GetOrderedNames());
            }

            for (var i = 0; i < dataFrame.RowCount; i++)
            {
                newDataFrame.AppendRow(dataFrame._data[i]);
            }



            return newDataFrame;
        }

        public DataFrame Merge(DataFrame dataFrame, MergeType mergeType)
        {
            return mergeType switch
            {
                MergeType.Horizontal => MergeColumns(dataFrame),
                MergeType.Vertical => MergeRows(dataFrame),
                _ => throw new ArgumentOutOfRangeException(nameof(mergeType), mergeType, null)
            };
        }

        /// <summary>
        /// Shuffle all rows.
        /// </summary>
        /// <returns>A new shuffled DataFrame</returns>
        public DataFrame Shuffle()
        {
            var randomizedDataFrame = new DataFrame(this);
            var random = new Random();

            for (var i = randomizedDataFrame.RowCount - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);
                (randomizedDataFrame._data[i], randomizedDataFrame._data[j]) =
                    (randomizedDataFrame._data[j], randomizedDataFrame._data[i]);
            }

            return randomizedDataFrame;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();

            // Print column names
            sb.Append(string.Join(", ", _columnNameMap.GetOrderedNames()));

            sb.Append('\n');

            // Print data

            for (var i = 0; i < RowCount; i++)
            {
                sb.Append(string.Join(", ", _data[i]));
                sb.Append('\n');
            }

            return sb.ToString();
        }

        # endregion

        // Private Methods

        # region Private Methods

        private void UpdateColumnCount(int count)
        {
            if (count <= ColumnCount) return;

            ColumnCount = count;
        }


        private string GetUniqueColumnName(string baseName)
        {
            var newName = baseName;
            var i = 1;
            while (_columnNameMap.ContainsName(newName))
            {
                newName = $"{baseName}.{i}";
                i++;
            }

            return newName;
        }


        private void SetPlaceholderColumnNames()
        {
            for (var i = 0; i < ColumnCount; i++)
            {
                _columnNameMap.TryAdd($"{GetUniqueColumnName("X")}", i);
            }
        }

        private void AdjustEmptyRow(int row)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new IndexOutOfRangeException("Row index out of range.");
            }

            if (ColumnCount <= _data[row].Count) return;

            for (var i = _data[row].Count; i < ColumnCount; i++)
            {
                _data[row].Add(null);
            }
        }


        private void AppendColumnNames(IReadOnlyList<string> columnNames)
        {
            if (columnNames.Count == 0)
                throw new ArgumentException("Column names cannot be empty.");

            for (var i = 0; i < columnNames.Count; i++)
            {
                _columnNameMap.TryAdd(GetUniqueColumnName(columnNames[i]), ColumnCount + i);
            }
        }


        private void AddEmptyRows(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _data.Add(new List<object?>());
            }
        }

        # endregion

        // Indexers

        # region Indexers

        public object? this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new IndexOutOfRangeException("Row index out of range.");
                }

                if (col < 0 || col >= ColumnCount)
                {
                    throw new IndexOutOfRangeException("Column index out of range.");
                }

                return _data[row][col];
            }
        }

        public object? this[int row, string header]
        {
            get
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new IndexOutOfRangeException("Row index out of range.");
                }

                // Check the index of the header
                if (!_columnNameMap.TryGetIndex(header, out var col))
                {
                    throw new ArgumentException("Header not found.");
                }

                return _data[row][col];
            }
        }

        # endregion
    }
}
