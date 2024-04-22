#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class DataSequence : ICloneable
    {
        private List<object?> _values;


        public IReadOnlyList<object?> Values => _values.AsReadOnly();
        public int Count => _values.Count;

        public bool IsEmpty => _values.Count == 0;
        public object? this[int index] => index < _values.Count ? _values[index] : default;

        public static DataSequence New(params object?[] values)
        {
            return new DataSequence(new List<object> { values });
        }

        public DataSequence()
        {
            _values = new List<object?>();
        }

        public DataSequence(DataSequence dataSequence)
        {
            _values = new List<object?>(dataSequence._values);
        }

        public DataSequence(IEnumerable<object?> values) : this()
        {
            Add(values);
        }

        public DataSequence(IEnumerable<string?> values) : this()
        {
            Add(values!.Cast<object?>());
        }

        public DataSequence(IEnumerable<float?> values) : this()
        {
            Add(values.Cast<object?>());
        }

        public DataSequence(IEnumerable<int?> values) : this()
        {
            Add(values.Cast<object?>());
        }



        public void Add(object? value)
        {
            _values.Add(value);
        }

        private void Add(IEnumerable<object?> values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }



        public object Clone()
        {
            return new DataSequence
            {
                _values = new List<object?>(_values),
            };
        }

        public override string ToString()
        {
            // Print like "Header1: Value1, Header2: Value2, ..."
            var sb = new StringBuilder();
            for (var i = 0; i < _values.Count; i++)
            {
                var value = _values[i] != null ? _values[i] : "NULL";
                sb.Append(value);
                if (i < _values.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }
    }

    public static class DataSequenceExtensions
    {
        public static DataSequence ToDataSequence(this IEnumerable<DataSequence> values)
        {
            var dataSequence = new DataSequence();
            foreach (var value in values)
            {
                dataSequence.Add(value);
            }

            return dataSequence;
        }

        public static IEnumerable<T> Cast<T>(this IEnumerable<object> values)
        {
            return Enumerable.Cast<T>(values);
        }
    }
}
