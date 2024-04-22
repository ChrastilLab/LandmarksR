#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// ReSharper disable InvalidXmlDocComment

namespace LandmarksR.Scripts.Experiment.Data
{
    public class DataFrame : IEnumerable<DataSequence>
    {
        private Dictionary<string, int> _headerToColumn = new();
        private readonly List<DataSequence> _rows = new();
        public int RowCount => _rows.Count;
        public int ColumnCount { get; private set; } = -1;

        public List<string> Headers => _headerToColumn.Keys.ToList();
        public List<int> HeaderIndices => _headerToColumn.Values.ToList();

        #region Constructors And Builders

        public DataFrame(params DataSequence[] dataSequences)
        {
            foreach (var dataSequence in dataSequences)
            {
                Add(dataSequence);
            }
        }

        public DataFrame(DataFrame dataFrame)
        {
            _headerToColumn = new Dictionary<string, int>(dataFrame._headerToColumn);
            _rows = new List<DataSequence>(dataFrame._rows);
            ColumnCount = dataFrame.ColumnCount;
        }

        public static DataFrame CreateRow(params object?[] values)
        {
            var dataSequence = new DataSequence();
            foreach (var value in values)
            {
                dataSequence.Add(value);
            }

            return new DataFrame { dataSequence };
        }

        public static DataFrame CreateColumn(params object?[] values)
        {
            var dataFrame = new DataFrame();
            foreach (var value in values)
            {
                dataFrame.Add(DataSequence.New(value));
            }

            return dataFrame;
        }

        public static DataFrame CreateColumn(string header, IEnumerable<object?> values)
        {
            var dataFrame = new DataFrame();
            dataFrame._headerToColumn[header] = dataFrame._headerToColumn.Count;
            foreach (var value in values)
            {
                dataFrame.Add(DataSequence.New(value));
            }

            return dataFrame;
        }

        public static DataFrame FromDictionary(Dictionary<string, List<object?>> data)
        {
            var dataFrame = new DataFrame();

            var headers = data.Keys.ToList();
            var values = data.Values.ToList();

            if (!CheckIfSameLength(values))
            {
                throw new ArgumentException("Values must have the same length.");
            }

            foreach (var header in headers)
            {
                dataFrame._headerToColumn[header] = dataFrame._headerToColumn.Count;
            }

            for (var i = 0; i < values[0].Count; ++i)
            {
                var dataSequence = new DataSequence();
                foreach (var value in values)
                {
                    dataSequence.Add(value[i]);
                }

                dataFrame.Add(dataSequence);
            }

            return dataFrame;
        }

        #endregion


        public void Add(DataSequence dataSequence)
        {
            _rows.Add(new DataSequence(dataSequence));

            if (ColumnCount == -1)
            {
                ColumnCount = dataSequence.Count;
                return;
            }

            UpdateColumnCount(dataSequence.Count);

            if (_headerToColumn.Count < ColumnCount)
            {
                for (var i = _headerToColumn.Count; i < ColumnCount; i++)
                {
                    var header = GetUniqueHeader("X");
                    _headerToColumn[header] = i;
                }
            }

        }

        public void Add(params object?[] values)
        {
            var dataSequence = new DataSequence(values);

            Add(dataSequence);
        }

        public void SetHeaders(List<string> headers)
        {
            if (headers == null) throw new ArgumentNullException(nameof(headers), "Headers cannot be null.");


            if (headers.Count < ColumnCount)
            {
                _headerToColumn = new Dictionary<string, int>();
                for (var i = 0; i < headers.Count; i++)
                {
                    _headerToColumn[headers[i]] = i;
                }

                for (var i = headers.Count; i < ColumnCount; i++)
                {
                    var header = GetUniqueHeader("X");
                    _headerToColumn[header] = i;
                }
            }

            else
            {
                UpdateColumnCount(headers.Count);
                _headerToColumn = new Dictionary<string, int>();
                for (var i = 0; i < ColumnCount; i++)
                {
                    var header = GetUniqueHeader(headers[i]);
                    _headerToColumn[header] = i;
                }
            }
        }

        private void UpdateColumnCount(int count)
        {
            ColumnCount = Math.Max(ColumnCount, count);
        }


        private DataSequence FillMissingValues(DataSequence dataSequence)
        {
            while (dataSequence.Count < ColumnCount)
            {
                dataSequence.Add(null);
            }

            return dataSequence;
        }

        public object? this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new IndexOutOfRangeException();
                }

                if (col < 0 || col >= ColumnCount)
                {
                    throw new IndexOutOfRangeException();
                }

                var r = _rows[row];
                return r.Count < ColumnCount ? null : r[col];
            }
        }

        public object? this[int row, string header]
        {
            get
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new IndexOutOfRangeException();
                }

                // _rows[row] = FillMissingValues(_rows[row]);

                if (_headerToColumn.TryGetValue(header, out var col))
                {
                    return _rows[row][col];
                }

                return null;
            }
        }

        public DataSequence GetRow(int row)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new IndexOutOfRangeException();
            }

            _rows[row] = FillMissingValues(_rows[row]);
            return _rows[row];
        }

        public DataSequence GetColumn(int col)
        {
            if (col < 0 || col >= ColumnCount)
            {
                throw new IndexOutOfRangeException();
            }

            var dataSequence = new DataSequence();
            foreach (var row in _rows)
            {
                if (col < row.Count)
                {
                    dataSequence.Add(row[col]);
                }
                else
                {
                    dataSequence.Add(null);
                }
            }

            return dataSequence;
        }

        public DataSequence GetColumn(string header)
        {
            if (!_headerToColumn.TryGetValue(header, out var col))
            {
                throw new ArgumentException("Header not found.");
            }


            return GetColumn(col);
        }

        public List<string> GetColumnHeaders()
        {
            return Headers;
        }

        public DataFrame Merge(DataFrame dataFrame)
        {
            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));

            if (dataFrame.RowCount != 0 && RowCount != 0 && dataFrame.RowCount != RowCount)
                throw new ArgumentException("DataFrames must have the same number of rows.");

            var newDataFrame = new DataFrame(this);

            foreach (var header in dataFrame.Headers)
            {
                if (!newDataFrame._headerToColumn.ContainsKey(header))
                {
                    // Shift the column index of the new header
                    newDataFrame._headerToColumn[header] =
                        newDataFrame.ColumnCount + dataFrame._headerToColumn[header];
                }
                else
                {
                    var newHeader = GetUniqueHeader(header);
                    newDataFrame._headerToColumn[newHeader] =
                        newDataFrame.ColumnCount + dataFrame._headerToColumn[header];
                }
            }

            // Add the new columns numbers
            newDataFrame.ColumnCount +=
                dataFrame.ColumnCount != -1 ? dataFrame.ColumnCount : 0;

            for (var i = 0; i < dataFrame.RowCount; i++)
            {
                // Fill missing values after update the autoColumnCount
                newDataFrame._rows[i] = FillMissingValues(newDataFrame._rows[i]);

                for (var j = 0; j < dataFrame.ColumnCount; j++)
                {
                    newDataFrame._rows[i].Add(dataFrame[i, j]);
                }
            }


            return newDataFrame;
        }

        private string GetUniqueHeader(string header)
        {
            var newHeader = header;
            var i = 1;
            while (_headerToColumn.ContainsKey(newHeader))
            {
                newHeader = $"{header}.{i}";
                i++;
            }

            return newHeader;
        }

        public DataFrame ConcatByColumn(IEnumerable<DataSequence> columns)
        {
            if (columns == null) throw new ArgumentNullException(nameof(columns));


            var combinedColumns = new List<DataSequence>(_rows);
            combinedColumns.AddRange(columns);


            if (!CheckIfSameLength(combinedColumns, out var length))
                throw new ArgumentException("Columns must have the same length.");

            var newDataFrame = new DataFrame();

            for (var i = 0; i < length; i++)
            {
                var dataSequence = new DataSequence();
                foreach (var column in combinedColumns)
                {
                    dataSequence.Add(column[i]);
                }

                newDataFrame.Add(dataSequence);
            }

            return newDataFrame;
        }

        public DataFrame ConcatByRow(DataFrame dataFrame)
        {
            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));

            var newDataFrame = new DataFrame();

            foreach (var row in this)
            {
                newDataFrame.Add(row);
            }

            foreach (var row in dataFrame)
            {
                newDataFrame.Add(row);
            }

            return this;
        }

        private static bool CheckIfSameLength(IReadOnlyList<List<object?>> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            var expectedLength = -1;
            foreach (var sequence in values)
            {
                if (expectedLength == -1)
                    expectedLength = sequence.Count;
                else if (expectedLength != sequence.Count)
                    return false;
            }

            if (expectedLength == -1)
                throw new ArgumentException("Values cannot be empty.");

            return true;
        }

        private static bool CheckIfSameLength(IEnumerable<DataSequence> dataSequences, out int expectedLength)
        {
            if (dataSequences == null) throw new ArgumentNullException(nameof(dataSequences));

            expectedLength = -1;
            foreach (var sequence in dataSequences)
            {
                if (expectedLength == -1)
                    expectedLength = sequence.Count;
                else if (expectedLength != sequence.Count)
                {
                    expectedLength = -1;
                    return false;
                }
            }

            if (expectedLength == -1)
                throw new ArgumentException("Values cannot be empty.");

            return true;
        }


        public IEnumerator<DataSequence> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < _headerToColumn.Count; i++)
            {
                sb.Append(Headers[i]);
                if (i < _headerToColumn.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append('\n');

            for (var i = 0; i < _rows.Count; i++)
            {
                _rows[i] = FillMissingValues(_rows[i]);

                sb.AppendLine(_rows[i].ToString());
            }

            return sb.ToString();
        }
    }


    public static class DataFrameExtension
    {
        public static DataFrame ConcatByRow(this IEnumerable<DataSequence> dataSequences)
        {
            if (dataSequences == null) throw new ArgumentNullException(nameof(dataSequences));
            var dataFrame = new DataFrame(dataSequences.ToArray());

            return dataFrame;
        }

        public static DataFrame ConcatByColumn(this IEnumerable<DataSequence> dataSequences)
        {
            return new DataFrame().ConcatByColumn(dataSequences);
        }

        public static DataFrame SelectRows(this DataFrame dataFrame, IEnumerable<int> rowIndices)
        {
            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));
            if (rowIndices == null) throw new ArgumentNullException(nameof(rowIndices));

            var dataSequences = rowIndices.Select(dataFrame.GetRow);
            return dataSequences.ConcatByRow();
        }

        public static DataFrame Shuffle(this DataFrame dataFrame)
        {
            if (dataFrame == null) throw new ArgumentNullException(nameof(dataFrame));

            var random = new System.Random();
            var dataSequences = dataFrame.OrderBy(_ => random.Next());
            return dataSequences.ConcatByRow();
        }
    }
}
