using System;
using System.Collections.Generic;
using System.Linq;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class DataEnumerator
    {
        private readonly List<DataFrame> _dataList = new();
        private int _position = -1;
        private readonly int _count;

        public DataEnumerator(DataFrame data, int minCount)
        {
            _dataList.Add(data);
            _count = minCount;
        }

        public DataEnumerator(DataFrame data)
        {
            _dataList.Add(data);
            _count = data.RowCount;
        }

        public DataEnumerator(List<DataFrame> dataList)
        {
            _dataList = dataList;
            _count = dataList.Select(df => df.RowCount).Prepend(int.MaxValue).Min(); // Get the minimum row count
        }

        public DataEnumerator(List<DataFrame> dataList, int minCount)
        {
            _dataList = dataList;
            _count = minCount;
        }

        public bool MoveNext()
        {
            _position++;
            return _position < _count;
        }

        public void Reset()
        {
            _position = -1;
        }

        public DataFrame GetCurrent()
        {
            if (_dataList.Count == 1)
                return _dataList[0].GetRow(_position);
            var row = new DataFrame();

            return _dataList.Aggregate(row, (current, df) => current.MergeColumns(df.GetRow(_position)));
        }

        public DataFrame GetCurrentByTable(int tableIndex)
        {
            return _dataList[tableIndex].GetRow(_position);
        }
    }
}
