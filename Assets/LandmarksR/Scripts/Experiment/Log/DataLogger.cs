using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Data;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class DataLogger : TextLogger
    {
        private readonly HashSet<string> _columns = new();
        private readonly Dictionary<string, string> _currentRow = new();
        private string _delimiter = ",";

        public void Begin(List<string> columns, string delimiter = ",")
        {
            _delimiter = delimiter;
            foreach (var column in columns)
            {
                _columns.Add(column);
            }
            LogData(string.Join(delimiter, columns));
        }

        public void SetValue(string column, string value)
        {
            if (!_columns.Contains(column))
            {
                throw new KeyNotFoundException($"Column {column} not found in initialized columns");
            }
            _currentRow[column] = value;
        }

        public void Log()
        {
            var row = _columns.Select(column => _currentRow.GetValueOrDefault(column, "")).ToList();
            LogData(string.Join(_delimiter, row));
            _currentRow.Clear();
        }

        public async void End()
        {
            await StopAsync();
        }
    }
}
