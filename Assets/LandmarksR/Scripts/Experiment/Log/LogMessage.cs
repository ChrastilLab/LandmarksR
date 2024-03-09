using System.Security;

namespace LandmarksR.Scripts.Experiment.Log
{
    public enum LogPriority
    {
        Info,
        Warning,
        Error,
    }
    public class LogMessage
    {
        public string FileName { get; set; }
        public LogPriority Priority { get;  }
        public string Message { get; }
        public string Tag { get; }

        public string Timestamp { get; }

        public LogMessage(string timestamp, string tag, LogPriority priority,string message)
        {
            Timestamp = timestamp;
            Tag = tag;
            Priority = priority;
            Message = message;
        }


        public override string ToString()
        {
            var priority = Priority switch
            {
                LogPriority.Info => "I",
                LogPriority.Warning => "W",
                LogPriority.Error => "E",
                _ => "UNKNOWN"
            };

            return $"{Timestamp}|{Tag}|{priority}|{Message}";
        }

        public string ToJson()
        {
            return $"{{\"filePath\":\"{FileName}\",\"message\":\"{SecurityElement.Escape(ToString())}\"}}";
        }

    }
}
