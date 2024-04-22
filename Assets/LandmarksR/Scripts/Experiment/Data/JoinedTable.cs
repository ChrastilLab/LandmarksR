using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class JoinedTable: Table
    {
        [SerializeField] private List<TextTable> tables;
        private DataFrame _joinedData = new();
        protected override void Prepare()
        {
            base.Prepare();
            Count = tables.Min(table => table.Count);

            foreach (var table in tables)
            {
                // TODO: Implement join operation for empty table and replace this
                if (_joinedData.RowCount == 0)
                {
                    _joinedData = table.Data;
                    continue;
                }
                _joinedData = _joinedData.Merge(table.Data);
            }

            Enumerator = new JoinedTableEnumerator(_joinedData, Count);
            TestIterate();
        }

        private void TestIterate()
        {
            Debug.Log("[Testing] Iterating through joined table.");
            while (Enumerator.MoveNext())
            {
                var current = Enumerator.GetCurrent();
                Debug.Log(current);
            }


            Enumerator.Reset();
        }

        private class JoinedTableEnumerator : IDataEnumerator
        {
            private readonly DataFrame _joinedData;
            private int _position = -1;
            private readonly int _count;

            public JoinedTableEnumerator(DataFrame joinedData, int minCount)
            {
                _joinedData = joinedData;
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
                var current = new DataFrame { _joinedData.GetRow(_position) };
                current.SetHeaders(_joinedData.GetColumnHeaders());
                return current;
            }
        }
    }
}
