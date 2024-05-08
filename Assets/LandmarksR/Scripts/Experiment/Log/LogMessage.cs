using System;
using System.Text;

namespace LandmarksR.Scripts.Experiment.Log
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
        Data
    }

    public class LogMessage
    {
        public LogType Type { get; }
        public string Message { get; }
        public string Tag { get; }
        public string Timestamp { get; }

        public LogMessage(string timestamp, string tag, LogType type, string message)
        {
            Timestamp = timestamp;
            Tag = tag;
            Type = type;
            Message = message;
        }

        public LogMessage(string message)
        {
            Type = LogType.Data;
            Message = message;
        }


        public override string ToString()
        {
            return Type switch
            {
                LogType.Info => $"{Timestamp}|{Tag}|I|{Message}",
                LogType.Warning => $"{Timestamp}|{Tag}|W|{Message}",
                LogType.Error => $"{Timestamp}|{Tag}|E|{Message}",
                LogType.Data => Message,
                _ => "UNKNOWN"
            };
        }

        public string ToJson(string fileName)
        {
            return
                $"{{\"filePath\":\"{fileName}\",\"message\":\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(ToString()))}\"}}";
        }
    }
}
