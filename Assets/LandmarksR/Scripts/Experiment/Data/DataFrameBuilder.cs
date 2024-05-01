#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class DataFrameBuilder
    {
        private readonly DataFrame _dataFrame = new();

        public DataFrameBuilder AddRow(params object?[] elements)
        {
            _dataFrame.AppendRow(elements.ToList());
            return this;
        }

        public DataFrameBuilder AddColumn(params object?[] elements)
        {
            if (elements.Length != _dataFrame.RowCount && _dataFrame.RowCount != 0)
            {
                throw new ArgumentException("Column length must be equal to the row count.");
            }

            _dataFrame.AppendColumn(elements.ToList(), "X");
            return this;
        }

        public DataFrameBuilder AddColumn(string header, List<object?> values)
        {
            _dataFrame.AppendColumn(values, header);
            return this;
        }

        public DataFrame Build()
        {
            return _dataFrame;
        }

        public static DataFrame CreateRow(params object?[] elements) => new DataFrameBuilder().AddRow(elements).Build();

        public static DataFrame CreateColumn(params object?[] elements) =>
            new DataFrameBuilder().AddColumn(elements).Build();

        public static DataFrame CreateColumn(string header, IEnumerable<object> values) =>
            new DataFrameBuilder().AddColumn(header, values).Build();

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
