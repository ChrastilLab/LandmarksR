using System;
using System.Collections.Generic;
using System.Linq;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class DataEnumerator
    {
        private readonly List<DataFrame> _dataList = new();
        private int _position = -1;
        public int Count { get; }
        private readonly MergeType _type;
        private readonly HashSet<int> _usedIndexes = new();
        private readonly int _randomSeed;
        private readonly Random _random;

        public DataEnumerator(DataFrame data, int randomSeed = 0)
        {
            _dataList.Add(data);
            Count = data.RowCount;
            _type = MergeType.Vertical;
            _randomSeed = randomSeed;
            if (_randomSeed > 0)
            {
                _random = new Random(_randomSeed);
            }
        }

        public DataEnumerator(List<DataFrame> dataList, MergeType type, int randomSeed = 0)
        {
            if (dataList.Count == 0)
            {
                throw new ArgumentException("Data list cannot be empty for enumeration.");
            }

            _type = type;
            _dataList = dataList;
            Count = type switch
            {
                MergeType.Horizontal =>
                    dataList.Select(df => df.RowCount).Prepend(int.MaxValue).Min() // Get the minimum row count
                ,
                MergeType.Vertical => dataList.Select(df => df.RowCount).Sum(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            _randomSeed = randomSeed;
            if (_randomSeed > 0)
            {
                _random = new Random(_randomSeed);
            }
        }

        public bool MoveNext()
        {
            _position++;
            return _position < Count;
        }

        public void Reset()
        {
            _position = -1;
            _usedIndexes.Clear();
        }

        public DataFrame GetCurrent()
        {

            var position = _position;
            if (_randomSeed > 0)
            {
                var randomIndex = _random.Next(0, Count);
                while (_usedIndexes.Contains(randomIndex))
                {
                    randomIndex = _random.Next(0, Count);
                }

                position = randomIndex;
                _usedIndexes.Add(randomIndex);
            }

            switch (_type)
            {
                case MergeType.Horizontal:
                    var row = new DataFrame();
                    return _dataList.Aggregate(row, (current, df) => current.Merge(df.GetRow(position), MergeType.Horizontal));
                case MergeType.Vertical:
                    return GetDataFrameAtIndex(_dataList, position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public DataFrame GetCurrentByTable(int tableIndex)
        {
            return _dataList[tableIndex].GetRow(_position);
        }

        private static DataFrame GetDataFrameAtIndex(List<DataFrame> dataList, int index)
        {
            // Check if the index is valid
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
            }

            var runningCount = 0;

            foreach (var dataframe in dataList)
            {
                if (dataframe != null)
                {
                    if (index < runningCount + dataframe.RowCount)
                    {
                        // The index is within the bounds of the current sublist
                        return dataframe.GetRow(index - runningCount);
                    }
                    runningCount += dataframe.RowCount;
                }
            }

            // If no sublist contains the index
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of the bounds of the combined lists.");
        }
    }
}
