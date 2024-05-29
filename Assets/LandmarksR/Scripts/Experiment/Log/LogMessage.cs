using System;
using System.Text;

using System;
using System.Text;

namespace LandmarksR.Scripts.Experiment.Log
{
    /// <summary>
    /// Represents the type of log message.
    /// </summary>
    public enum LogType
    {
        Info,
        Warning,
        Error,
        Data
    }

    /// <summary>
    /// Represents a log message.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Gets the type of the log message.
        /// </summary>
        public LogType Type { get; }

        /// <summary>
        /// Gets the message content.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the tag associated with the message.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Gets the timestamp of the message.
        /// </summary>
        public string Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the LogMessage class.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message.</param>
        /// <param name="tag">The tag associated with the message.</param>
        /// <param name="type">The type of the log message.</param>
        /// <param name="message">The message content.</param>
        public LogMessage(string timestamp, string tag, LogType type, string message)
        {
            Timestamp = timestamp;
            Tag = tag;
            Type = type;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the LogMessage class for data messages.
        /// </summary>
        /// <param name="message">The message content.</param>
        public LogMessage(string message)
        {
            Type = LogType.Data;
            Message = message;
        }

        /// <summary>
        /// Returns a string representation of the log message.
        /// </summary>
        /// <returns>A string representing the log message.</returns>
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

        /// <summary>
        /// Converts the log message to a JSON string.
        /// </summary>
        /// <param name="fileName">The file name associated with the log message.</param>
        /// <returns>A JSON string representing the log message.</returns>
        public string ToJson(string fileName)
        {
            return
                $"{{\"filePath\":\"{fileName}\",\"message\":\"{Convert.ToBase64String(Encoding.UTF8.GetBytes(ToString()))}\"}}";
        }
    }
}
