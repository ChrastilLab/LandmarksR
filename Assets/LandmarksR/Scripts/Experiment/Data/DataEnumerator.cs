using System;
using System.Collections.Generic;
using System.Linq;

namespace LandmarksR.Scripts.Experiment.Data
{
    /// <summary>
    /// Enumerator for iterating over DataFrames.
    /// </summary>
    public class DataEnumerator
    {
        private readonly List<DataFrame> _dataList;
        private int _position = -1;
        public int Count { get; }
        private readonly MergeType _type;
        private readonly int _randomSeed;
        private readonly List<int> _randomizedIndexes;

        /// <summary>
        /// Initializes a new instance of the DataEnumerator class for a single DataFrame.
        /// </summary>
        /// <param name="data">The DataFrame to enumerate.</param>
        /// <param name="randomSeed">The seed for randomization (default is 0).</param>
        public DataEnumerator(DataFrame data, int randomSeed = 0)
        {
            _dataList = new List<DataFrame> { data };
            Count = data.RowCount;
            _type = MergeType.Vertical;
            _randomSeed = randomSeed;
            if (_randomSeed > 0)
            {
                var random = new Random(_randomSeed);
                _randomizedIndexes = Enumerable.Range(0, Count).OrderBy(x => random.Next()).ToList();
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataEnumerator class for a list of DataFrames.
        /// </summary>
        /// <param name="dataList">The list of DataFrames to enumerate.</param>
        /// <param name="type">The type of merge (horizontal or vertical).</param>
        /// <param name="randomSeed">The seed for randomization (default is 0).</param>
        /// <exception cref="ArgumentException">Thrown if the data list is empty.</exception>
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
                MergeType.Horizontal => dataList.Select(df => df.RowCount).Prepend(int.MaxValue).Min(),
                MergeType.Vertical => dataList.Select(df => df.RowCount).Sum(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            _randomSeed = randomSeed;
            if (_randomSeed > 0)
            {
                var random = new Random(_randomSeed);
                _randomizedIndexes = Enumerable.Range(0, Count).OrderBy(x => random.Next()).ToList();
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            _position++;
            return _position < Count;
        }

        /// <summary>
        /// Resets the enumerator to its initial position.
        /// </summary>
        public void Reset()
        {
            _position = -1;
        }

        /// <summary>
        /// Gets the current DataFrame row at the enumerator's position.
        /// </summary>
        /// <returns>The current DataFrame row.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the enumerator is out of range.</exception>
        public DataFrame GetCurrent()
        {
            var position = _position;
            if (_randomSeed > 0)
            {
                position = _randomizedIndexes[_position];
            }

            return _type switch
            {
                MergeType.Horizontal => GetMergedRow(position),
                MergeType.Vertical => GetDataFrameAtIndex(_dataList, position),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Gets the current DataFrame row by table index.
        /// </summary>
        /// <param name="tableIndex">The index of the table.</param>
        /// <returns>The current DataFrame row by table index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the enumerator is out of range.</exception>
        public DataFrame GetCurrentByTable(int tableIndex)
        {
            var position = _position;
            if (_randomSeed > 0)
            {
                position = _randomizedIndexes[_position];
            }
            return _dataList[tableIndex].GetRow(position);
        }

        /// <summary>
        /// Gets the merged DataFrame row of all tables at the specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private DataFrame GetMergedRow(int position)
        {
            var row = new DataFrame();
            return _dataList.Aggregate(row, (current, df) => current.Merge(df.GetRow(position), MergeType.Horizontal));
        }

        /// <summary>
        /// Gets the DataFrame at the specified index.
        /// The index should be the correct index in the merged list
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static DataFrame GetDataFrameAtIndex(List<DataFrame> dataList, int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
            }

            var runningCount = 0;

            foreach (var dataframe in dataList)
            {
                if (index < runningCount + dataframe.RowCount)
                {
                    return dataframe.GetRow(index - runningCount);
                }
                runningCount += dataframe.RowCount;
            }

            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of the bounds of the combined lists.");
        }
    }
}
