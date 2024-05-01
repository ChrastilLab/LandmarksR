using System;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    [Serializable]
    public class DelimiterOption
    {
        public int delimiterIndex;

        public string customValue;

        public string Value => delimiterIndex switch
        {
            0 => ",",
            1 => "\t",
            2 => ";",
            3 => " ",
            4 => customValue,
            _ => ","
        };

        public static readonly string[] Options = { "Comma", "Tab", "Semicolon", "Space", "Custom" };
    }

    public class TextTable : Table
    {
        [SerializeField] private bool randomize;
        [SerializeField] private List<string> rows;
        [SerializeField] private bool hasHeader = true;
        [SerializeField] private List<string> headers;
        [SerializeField] private DelimiterOption delimiterOption;
        [SerializeField] private string dataPath;
        [SerializeField] private string indexesToExclude;

        public override int Count => Data.RowCount;

        public IReadOnlyList<string> StringRows => rows;

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

        public void AppendStringRows(List<string> stringRows)
        {
            var newRows = new List<string>();
            newRows.AddRange(rows);
            newRows.AddRange(stringRows);
            rows = newRows;
        }

        private void Parse()
        {
            if (rows is null || rows.Count == 0)
            {
                ExperimentLogger.Instance.W("data", $"({name}) Rows are empty.");
                Data = new DataFrame();
                return;
            }
            try
            {
                var counter = 0;
                if (string.IsNullOrEmpty(delimiterOption.Value))
                {
                    ExperimentLogger.Instance.E("data", "Delimiter is not set.");
                    return;
                }

                if (!string.IsNullOrEmpty(indexesToExclude) || !string.IsNullOrWhiteSpace(indexesToExclude))
                {
                    RemoveBySlice(indexesToExclude);
                }

                foreach (var values in rows.Select(row => row.Split(delimiterOption.Value).Cast<object>().ToList()))
                {
                    Data.AppendRow(values);
                    counter++;
                }

                if (headers is { Count: > 0 })
                    Data.SetColumnNames(headers);


                if (randomize)
                {
                    var random = new System.Random();
                    // TODO: Implement randomization
                    ExperimentLogger.Instance.I("data", $"Data is randomized.");
                }

                ExperimentLogger.Instance.I("data", $"{counter} rows are created");
                Enumerator = new DataEnumerator(Data);
            }
            catch (Exception e)
            {
                ExperimentLogger.Instance.E("data",
                    $"<{name}> Error parsing data: {e.Message}\n Check your header, rows and delimiter settings.");
            }
        }

        private void RemoveBySlice(string slice)
        {
            if (string.IsNullOrEmpty(slice))
                return;

            // Split the slice string by commas to handle multiple slices
            var slices = slice.Split(',');

            // List to hold the ranges to be removed
            var removals = new List<(int start, int count)>();

            foreach (var singleSlice in slices)
            {
                // Parse each slice part
                int start = 0, end = rows.Count;
                bool startSet = false, endSet = false;

                var parts = singleSlice.Split(':');
                if (parts.Length == 2)
                {
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        start = Convert.ToInt32(parts[0]);
                        startSet = true;
                    }

                    if (!string.IsNullOrEmpty(parts[1]))
                    {
                        end = Convert.ToInt32(parts[1]);
                        endSet = true;
                    }
                }

                // Adjust negative indices
                if (start < 0)
                    start = rows.Count + start;
                if (end < 0)
                    end = rows.Count + end;

                // Ensure indices are within bounds
                start = Math.Max(start, 0);
                end = Math.Min(end, rows.Count);

                if (startSet && endSet)
                {
                    removals.Add((start, end - start));
                }
                else if (startSet)
                {
                    removals.Add((start, rows.Count - start));
                }
                else if (endSet)
                {
                    removals.Add((0, end));
                }
            }

            // Sort removal ranges by starting index in descending order to avoid shifting issues
            removals.Sort((a, b) => b.start.CompareTo(a.start));

            // Remove the calculated ranges
            foreach (var removal in removals)
            {
                if (removal.start < rows.Count) // Check needed if previous removals make indices out of range
                    rows.RemoveRange(removal.start, Math.Min(removal.count, rows.Count - removal.start));
            }
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
                        headers = line.Split(delimiterOption.Value).ToList();
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
                Logger.E("data", $"Error reading file: {ex.Message}");
            }
        }

        public void SaveToFile()
        {
            try
            {
                // Using StreamWriter to write to a file
                using var writer = new System.IO.StreamWriter(dataPath + "_output.txt");

                // Writing headers if they exist
                if (headers is { Count: > 0 })
                {
                    writer.WriteLine(string.Join(delimiterOption.Value, headers));
                }

                // Writing rows
                foreach (var row in rows)
                {
                    writer.WriteLine(row);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
