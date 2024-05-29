using System;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Tasks;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    /// <summary>
    /// Represents a table that merges multiple tables based on a specified merge type.
    /// </summary>
    public class MergedTable : Table
    {
        /// <summary>
        /// List of tables to merge.
        /// </summary>
        [SerializeField] private List<Table> tables;

        /// <summary>
        /// The type of merge (horizontal or vertical).
        /// </summary>
        [SerializeField] private MergeType mergeType = MergeType.Horizontal;

        /// <summary>
        /// If true, tables will be merged into a new data table, potentially using more RAM.
        /// </summary>
        [Tooltip("If true, the tables will be merged as a new data table, which may result in more RAM usage")]
        [SerializeField] private bool hardMerge;

        /// <summary>
        /// Indicates if the rows should be randomized.
        /// </summary>
        [SerializeField] private bool randomize;

        /// <summary>
        /// List of debug rows for verification purposes.
        /// </summary>
        [SerializeField] private List<string> debugRows;

        /// <summary>
        /// Prepares the merged table by merging the specified tables.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Functional);
            base.Prepare();
            if (tables == null || tables.Count == 0)
            {
                Logger.W("data", "No tables to merge");
                return;
            }

            var randomSeed = randomize ? DateTime.Now.Millisecond : 0;
            Logger.I("data", "Random seed: " + randomSeed);

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

        /// <summary>
        /// Updates the debug rows for verification purposes.
        /// </summary>
        private void UpdateDebugRows()
        {
            var results = new List<string>();
            while (Enumerator.MoveNext())
            {
                results.Add(string.Join(", ", Enumerator.GetCurrent().GetRawRow(0)));
            }
            debugRows = results;
            Enumerator.Reset();
        }
    }
}
