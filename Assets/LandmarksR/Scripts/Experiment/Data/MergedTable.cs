using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{


    public class MergedTable: Table
    {
        [SerializeField] private List<Table> tables;
        [SerializeField] private MergeType mergeType = MergeType.Horizontal;

        [Tooltip("If true, the tables will be merged as a new data table, which may result in more RAM usage")]
        [SerializeField] private bool hardMerge;
        [SerializeField] private bool randomize;
        [SerializeField] private List<string> debugRows;


        protected override void Prepare()
        {
            base.Prepare();
            if (tables == null || tables.Count == 0)
            {
                Logger.W("data", "No tables to merge");
                return;
            }

            var randomSeed = randomize ? DateTime.Now.Millisecond : 0;

            if (hardMerge)
            {
                foreach (var table in tables)
                {
                    Logger.I("data", "Before merge: " + table.Data.GetRow(0));
                    Data = Data.Merge(table.Data, mergeType);
                }

                Logger.I("data", Data.GetRow(0));

                Enumerator = new DataEnumerator(Data, randomSeed);
                Count = Data.RowCount;
            }
            else
            {
                var dataList = tables.Select(table => table.Data).ToList();

                Enumerator = new DataEnumerator(dataList, mergeType, randomSeed);
            }

            Count = Enumerator.Count;

            UpdateDebugRows();
        }

        private void UpdateDebugRows()
        {
           var results = new List<string>();
           while (Enumerator.MoveNext())
           {
                results.Add(string.Join(", ",Enumerator.GetCurrent().GetRawRow(0)));
           }
           debugRows = results;
           Enumerator.Reset();
        }
    }
}
