using System;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{

    public class TextTable : Table
    {

        [SerializeField] private bool randomize;
        [SerializeField] private List<string> rows;
        [SerializeField] private bool hasHeader = true;
        [SerializeField] private List<string> headers;
        [SerializeField] private string delimiter = ",";
        [SerializeField] private string dataPath;
        [SerializeField] private List<int> debugSelectedRows;

        public DataFrame Data = new();
        public override int Count => Data.RowCount;

        protected override void Prepare()
        {
            base.Prepare();
            Parse();
        }

        public void SetHeaders(List<string> stringHeader)
        {
            headers = stringHeader;
        }

        public void SetStringRows(List<string> stringRows)
        {
            rows = stringRows;
        }

        private void Parse()
        {
            var counter = 0;
            if (string.IsNullOrEmpty(delimiter))
            {
                ExperimentLogger.Instance.E("data", "Delimiter is not set.");
                return;
            }


            if (headers is { Count: > 0 })
                Data.SetHeaders(headers);

            foreach (var values in rows.Select(row => row.Split(delimiter).Cast<object>().ToList()))
            {
                Data.Add(new DataSequence(values));
                counter++;
            }


            if (debugSelectedRows.Count > 0)
            {
                Data = Data.Where((_, i) => debugSelectedRows.Contains(i)).ConcatByRow();
                ExperimentLogger.Instance.I("data",$"Selected rows are {string.Join(",", debugSelectedRows)}");
            }

            if (randomize)
            {
                var random = new System.Random();
                Data = Data.OrderBy(_ => random.Next()).ConcatByRow();
                ExperimentLogger.Instance.I("data",$"Data is randomized.");
            }

            ExperimentLogger.Instance.I("data",$"{counter} rows are created");
            Enumerator = new TextTableEnumerator(Data);
        }



        public void LoadFromFile()
        {
            if (string.IsNullOrWhiteSpace(dataPath))
            {
                // Log error or handle the case where dataPath is not set
                ExperimentLogger.Instance.E("data", "Data path is not set.");
                return;
            }

            try
            {
                // Using IEnumerable<string> to lazily read lines for memory efficiency
                var lines = System.IO.File.ReadLines(dataPath);

                // Initializing rows list to ensure it's always a valid object
                rows = new List<string>();

                var isFirstLine = true;
                foreach (var line in lines)
                {
                    // If the first line is a header, process it separately
                    if (isFirstLine && hasHeader)
                    {
                        headers = line.Split(delimiter).ToList();
                        isFirstLine = false;
                        continue;
                    }

                    rows.Add(line);
                }

                // if (rows.Count == 0)
                // {
                //     // Handle case where file is empty or only contains headers
                //     return;
                // }
            }
            catch (System.IO.IOException ex)
            {
                // Log the exception or handle it as needed
                ExperimentLogger.Instance.E($"data", $"Error reading file: {ex.Message}");
            }
        }

        private class TextTableEnumerator : IDataEnumerator
        {
            private readonly IEnumerator<DataSequence> _enumerator;

            public TextTableEnumerator(DataFrame data)
            {
                _enumerator = data.GetEnumerator();
            }
            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public DataFrame GetCurrent()
            {
                return new DataFrame(_enumerator.Current);
            }
        }
    }
}
