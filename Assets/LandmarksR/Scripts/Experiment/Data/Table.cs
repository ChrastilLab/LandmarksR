using System;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class Row
    {
        private List<string> _values;
        private Dictionary<string, string> _headerValuePairs;

        public static bool TryCreate(List<string> values, IReadOnlyList<string> headers, out Row row)
        {
            row = new Row();
            if (values.Count != headers.Count)
            {
                DebugLogger.Instance.E("data","Values and headers count does not match.");
                return false;
            }

            row._values = values;
            row._headerValuePairs = new Dictionary<string, string>();

            for (var i = 0; i < headers.Count; i++)
            {
                row._headerValuePairs.Add(headers[i], values[i]);
            }

            return true;
        }

        public string GetValue(string header)
        {
            if (!_headerValuePairs.ContainsKey(header))
            {
                DebugLogger.Instance.E("data", $"Header {header} does not exist.");
                return null;
            }

            return _headerValuePairs[header];
        }
    }
    public class Table : MonoBehaviour
    {
        [SerializeField] private List<string> rows;
        [SerializeField] private bool hasHeader = true;
        [SerializeField] private List<string> headers;
        [SerializeField] private string delimiter = ",";
        [SerializeField] private string dataPath;

        private readonly List<Row> _rows = new();
        private int _currentRowIndex;


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            var counter = 0;
            if (string.IsNullOrEmpty(delimiter) || string.IsNullOrWhiteSpace(delimiter))
            {
                DebugLogger.Instance.E("Delimiter is not set.", "data");
                return;
            }

            foreach (var values in rows.Select(stringRow => stringRow.Split(delimiter).ToList()))
            {
                if (Row.TryCreate(values, headers, out var row))
                {
                    _rows.Add(row);
                    counter++;
                }
                else
                {
                    DebugLogger.Instance.W($"Row {counter} is not created.", "data");
                }
            }

            DebugLogger.Instance.I($"{counter} rows are created.", "data");

        }

        public bool HasNextRow()
        {
            return _currentRowIndex < _rows.Count;
        }

        public void MoveNext()
        {
            if (!HasNextRow())
            {
                return;
            }
            _currentRowIndex++;
        }

        public Row GetCurrentRow()
        {
            return _rows[_currentRowIndex];
        }

        public string GetValue(string header)
        {
            return GetCurrentRow().GetValue(header);
        }

        public int GetRowCount()
        {
            return _rows.Count;
        }

        public void ResetCurrentRow()
        {
            _currentRowIndex = 0;
        }

        public void LoadFromFile()
        {
            if (string.IsNullOrWhiteSpace(dataPath))
            {
                // Log error or handle the case where dataPath is not set
                DebugLogger.Instance.E("data", "Data path is not set.");
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

                if (rows.Count == 0)
                {
                    // Handle case where file is empty or only contains headers
                    return;
                }
            }
            catch (System.IO.IOException ex)
            {
                // Log the exception or handle it as needed
                DebugLogger.Instance.E($"data", $"Error reading file: {ex.Message}");
            }
        }


    }
}
