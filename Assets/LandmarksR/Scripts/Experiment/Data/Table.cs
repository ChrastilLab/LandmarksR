using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Data
{
    public class Table : MonoBehaviour
    {
        [SerializeField] private List<string> rows;
        [SerializeField] private bool hasHeader = true;
        [SerializeField] private List<string> headers;
        [SerializeField] private string delimiter = ",";
        [SerializeField] private string dataPath;

        private void Start()
        {
        }

        public void LoadFromFile()
        {
            if (string.IsNullOrWhiteSpace(dataPath))
            {
                // Log error or handle the case where dataPath is not set
                throw new ArgumentException("Data path is not provided.");
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
                throw new ApplicationException($"Error reading from {dataPath}", ex);
            }
        }


    }
}
