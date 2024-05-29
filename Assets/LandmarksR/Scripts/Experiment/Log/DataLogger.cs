using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Data;

namespace LandmarksR.Scripts.Experiment.Log
{
    /// <summary>
    /// Manages logging of data rows with specific columns.
    /// </summary>
    public class DataLogger : TextLogger
    {
        private readonly HashSet<string> _columns = new();
        private readonly Dictionary<string, string> _currentRow = new();
        private string _delimiter = ",";

        /// <summary>
        /// Begins logging a new data set with specified columns and delimiter.
        /// </summary>
        /// <param name="columns">The columns for the data set.</param>
        /// <param name="delimiter">The delimiter to use between columns.</param>
        public void Begin(List<string> columns, string delimiter = ",")
        {
            _delimiter = delimiter;
            foreach (var column in columns)
            {
                _columns.Add(column);
            }
            LogData(string.Join(delimiter, columns));
        }

        /// <summary>
        /// Sets a value for a specific column in the current row.
        /// </summary>
        /// <param name="column">The column to set the value for.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the column is not found in the initialized columns.</exception>
        public void SetValue(string column, string value)
        {
            if (!_columns.Contains(column))
            {
                throw new KeyNotFoundException($"Column {column} not found in initialized columns");
            }
            _currentRow[column] = value;
        }

        /// <summary>
        /// Logs the current row of data.
        /// </summary>
        public void Log()
        {
            var row = _columns.Select(column => _currentRow.GetValueOrDefault(column, "")).ToList();
            LogData(string.Join(_delimiter, row));
            _currentRow.Clear();
        }

        /// <summary>
        /// Ends the logging of the current data set.
        /// </summary>
        public async void End()
        {
            await StopAsync();
        }
    }
}
