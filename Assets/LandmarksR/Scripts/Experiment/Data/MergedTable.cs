using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    // TODO: Implement join operation for rows
    public class MergedTable: Table
    {
        [SerializeField] private List<TextTable> tables;
        protected override void Prepare()
        {
            base.Prepare();
            Count = tables.Min(table => table.Count);
            var data = tables.Select(table => table.Data).ToList();
            Enumerator = new DataEnumerator(data, Count);
            // TestIterate();
            // TestMerge();
        }

        private void TestIterate()
        {
            Logger.I("data", "Iterate Joined Table");
            while (Enumerator.MoveNext())
            {
                var current = Enumerator.GetCurrent();
                Logger.I("data", current.ToString());
            }

            Enumerator.Reset();
        }

        private void TestMerge()
        {
        }


    }
}
