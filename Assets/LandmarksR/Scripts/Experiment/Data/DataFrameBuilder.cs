#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandmarksR.Scripts.Experiment.Data
{
    /// <summary>
    /// Builder class for constructing DataFrame objects.
    /// </summary>
    public class DataFrameBuilder
    {
        private readonly DataFrame _dataFrame = new();

        /// <summary>
        /// Adds a row to the DataFrame.
        /// </summary>
        /// <param name="elements">The elements of the row.</param>
        /// <returns>The current DataFrameBuilder instance.</returns>
        public DataFrameBuilder AddRow(params object?[] elements)
        {
            _dataFrame.AppendRow(elements.ToList());
            return this;
        }

        /// <summary>
        /// Adds a column to the DataFrame.
        /// </summary>
        /// <param name="elements">The elements of the column.</param>
        /// <returns>The current DataFrameBuilder instance.</returns>
        /// <exception cref="ArgumentException">Thrown if the column length does not match the row count.</exception>
        public DataFrameBuilder AddColumn(params object?[] elements)
        {
            if (elements.Length != _dataFrame.RowCount && _dataFrame.RowCount != 0)
            {
                throw new ArgumentException("Column length must be equal to the row count.");
            }

            _dataFrame.AppendColumn(elements.ToList(), "X");
            return this;
        }

        /// <summary>
        /// Adds a column with a specified header to the DataFrame.
        /// </summary>
        /// <param name="columnName">The header of the column.</param>
        /// <param name="values">The values of the column.</param>
        /// <returns>The current DataFrameBuilder instance.</returns>
        public DataFrameBuilder AddColumn(string columnName, List<object?> values)
        {
            _dataFrame.AppendColumn(values, columnName);
            return this;
        }

        /// <summary>
        /// Builds and returns the constructed DataFrame.
        /// </summary>
        /// <returns>The constructed DataFrame.</returns>
        public DataFrame Build()
        {
            return _dataFrame;
        }

        /// <summary>
        /// Creates a DataFrame with a single row.
        /// </summary>
        /// <param name="elements">The elements of the row.</param>
        /// <returns>A DataFrame containing the specified row.</returns>
        public static DataFrame CreateRow(params object?[] elements) => new DataFrameBuilder().AddRow(elements).Build();

        /// <summary>
        /// Creates a DataFrame with a single column.
        /// </summary>
        /// <param name="elements">The elements of the column.</param>
        /// <returns>A DataFrame containing the specified column.</returns>
        public static DataFrame CreateColumn(params object?[] elements) =>
            new DataFrameBuilder().AddColumn(elements).Build();

        /// <summary>
        /// Creates a DataFrame with a single column and a specified header.
        /// </summary>
        /// <param name="header">The header of the column.</param>
        /// <param name="values">The values of the column.</param>
        /// <returns>A DataFrame containing the specified column.</returns>
        public static DataFrame CreateColumn(string header, IEnumerable<object> values) =>
            new DataFrameBuilder().AddColumn(header, values.ToList()).Build();

        /// <summary>
        /// Creates a DataFrame from a dictionary of column headers and values.
        /// </summary>
        /// <param name="data">The dictionary containing column headers and values.</param>
        /// <returns>A DataFrame constructed from the specified dictionary.</returns>
        /// <exception cref="ArgumentException">Thrown if the values do not have the same length.</exception>
        public static DataFrame FromDictionary(Dictionary<string, List<object?>> data)
        {
            var dataFrame = new DataFrame();

            var values = data.Values.ToList();

            if (!DataUtilities.CheckEqualLength(values, out _))
            {
                throw new ArgumentException("Values must have the same length (row number).");
            }

            foreach (var (header, value) in data)
            {
                dataFrame.AppendColumn(value, header);
            }

            return dataFrame;
        }
    }
}
