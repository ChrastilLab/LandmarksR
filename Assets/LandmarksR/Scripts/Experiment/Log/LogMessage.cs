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
        private LogPriority Priority { get;  }
        private string Message { get; }
        private string Tag { get; }
        private string Timestamp { get; }

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

        public string ToJson(string fileName)
        {
            return $"{{\"filePath\":\"{fileName}\",\"message\":\"{SecurityElement.Escape(ToString())}\"}}";
        }

    }
}
