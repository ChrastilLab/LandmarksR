using System;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Log;
using LandmarksR.Scripts.Experiment.Tasks;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    /// <summary>
    /// Represents delimiter options for separating values in a text table.
    /// </summary>
    [Serializable]
    public class DelimiterOption
    {
        /// <summary>
        /// The index of the delimiter.
        /// </summary>
        public int delimiterIndex;

        /// <summary>
        /// Custom delimiter value.
        /// </summary>
        public string customValue;

        /// <summary>
        /// Gets the delimiter value based on the selected index.
        /// </summary>
        public string Value => delimiterIndex switch
        {
            0 => ",",
            1 => "\t",
            2 => ";",
            3 => " ",
            4 => customValue,
            _ => ","
        };

        /// <summary>
        /// Available delimiter options.
        /// </summary>
        public static readonly string[] Options = { "Comma", "Tab", "Semicolon", "Space", "Custom" };
    }

    /// <summary>
    /// Represents a text table in the experiment.
    /// </summary>
    public class TextTable : Table
    {
        /// <summary>
        /// Indicates if the rows should be randomized.
        /// </summary>
        [SerializeField] private bool randomize;

        /// <summary>
        /// List of rows in the table.
        /// </summary>
        [SerializeField] private List<string> rows;

        /// <summary>
        /// Indicates if the table has a header.
        /// </summary>
        [SerializeField] private bool hasHeader = true;

        /// <summary>
        /// List of headers in the table.
        /// </summary>
        [SerializeField] private List<string> headers;

        /// <summary>
        /// Delimiter option for the table.
        /// </summary>
        [SerializeField] private DelimiterOption delimiterOption;

        /// <summary>
        /// Path to the data file.
        /// </summary>
        [SerializeField] private string dataPath;

        /// <summary>
        /// Indexes to exclude from the table.
        /// </summary>
        [SerializeField] private string indexesToExclude;

        /// <summary>
        /// Gets the count of rows in the table.
        /// </summary>
        public override int Count => Data.RowCount;

        /// <summary>
        /// Gets the rows in the table as a read-only list.
        /// </summary>
        public IReadOnlyList<string> StringRows => rows;

        /// <summary>
        /// Prepares the table by parsing the data.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Functional);
            base.Prepare();
            Parse();
        }

        /// <summary>
        /// Sets the headers for the table.
        /// </summary>
        /// <param name="stringHeader">The headers to set.</param>
        public void SetHeaders(List<string> stringHeader)
        {
            headers = stringHeader;
        }

        /// <summary>
        /// Sets the rows for the table.
        /// </summary>
        /// <param name="stringRows">The rows to set.</param>
        public void SetStringRows(List<string> stringRows)
        {
            rows = stringRows;
        }

        /// <summary>
        /// Appends rows to the existing rows in the table.
        /// </summary>
        /// <param name="stringRows">The rows to append.</param>
        public void AppendStringRows(List<string> stringRows)
        {
            rows.AddRange(stringRows);
        }

        /// <summary>
        /// Parses the rows in the table based on the delimiter option.
        /// </summary>
        private void Parse()
        {
            if (rows == null || rows.Count == 0)
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

                if (!string.IsNullOrEmpty(indexesToExclude))
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
                    $"<{name}> Error parsing data: {e.Message}\n Check your header, rows, and delimiter settings.");
            }
        }

        /// <summary>
        /// Removes rows from the table based on a slice string.
        /// </summary>
        /// <param name="slice">The slice string specifying the rows to remove.</param>
        private void RemoveBySlice(string slice)
        {
            if (string.IsNullOrEmpty(slice)) return;

            var slices = slice.Split(',');

            var removals = new List<(int start, int count)>();

            foreach (var singleSlice in slices)
            {
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

                if (start < 0) start = rows.Count + start;
                if (end < 0) end = rows.Count + end;

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

            removals.Sort((a, b) => b.start.CompareTo(a.start));

            foreach (var removal in removals)
            {
                if (removal.start < rows.Count)
                    rows.RemoveRange(removal.start, Math.Min(removal.count, rows.Count - removal.start));
            }
        }

        /// <summary>
        /// Loads data from a file into the table.
        /// </summary>
        public void LoadFromFile()
        {
            if (string.IsNullOrWhiteSpace(dataPath))
            {
                ExperimentLogger.Instance.E("data", "Data path is not set.");
                return;
            }

            try
            {
                var lines = System.IO.File.ReadLines(dataPath);
                rows = new List<string>();

                var isFirstLine = true;
                foreach (var line in lines)
                {
                    if (isFirstLine && hasHeader)
                    {
                        headers = line.Split(delimiterOption.Value).ToList();
                        isFirstLine = false;
                        continue;
                    }

                    rows.Add(line);
                }
            }
            catch (System.IO.IOException ex)
            {
                ExperimentLogger.Instance.E("data", $"Error reading file: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the data from the table to a file.
        /// </summary>
        public void SaveToFile()
        {
            try
            {
                using var writer = new System.IO.StreamWriter(dataPath + "_output.txt");

                if (headers is { Count: > 0 })
                {
                    writer.WriteLine(string.Join(delimiterOption.Value, headers));
                }

                foreach (var row in rows)
                {
                    writer.WriteLine(row);
                }
            }
            catch (Exception e)
            {
                ExperimentLogger.Instance.E("data", $"Error writing file: {e.Message}");
            }
        }
    }
}
