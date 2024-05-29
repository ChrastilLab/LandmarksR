using System;
using System.Threading.Tasks;

namespace LandmarksR.Scripts.Experiment.Log
{
    /// <summary>
    /// Manages logging both locally and remotely.
    /// </summary>
    public class TextLogger
    {
        private LocalLogger _localLogger;
        private RemoteLogger _remoteLogger;

        /// <summary>
        /// Enables local logging to a specified file.
        /// </summary>
        /// <param name="filePath">The path to the log file.</param>
        public void EnableLocalLog(string filePath = "log.txt")
        {
            _localLogger = new LocalLogger(filePath);
        }

        /// <summary>
        /// Enables remote logging to specified URLs.
        /// </summary>
        /// <param name="filePath">The path to the log file.</param>
        /// <param name="remoteStatusUrl">The URL for remote status.</param>
        /// <param name="remoteLogUrl">The URL for remote logging.</param>
        public void EnableRemoteLog(string filePath = "log.txt",
            string remoteStatusUrl = "http://localhost:3000/status", string remoteLogUrl = "http://localhost:3000/log")
        {
            _remoteLogger = new RemoteLogger(filePath, remoteStatusUrl, remoteLogUrl);
        }

        /// <summary>
        /// Logs a message with a specified tag and type.
        /// </summary>
        /// <param name="messageTag">The tag associated with the message.</param>
        /// <param name="type">The type of the log message.</param>
        /// <param name="message">The message to log.</param>
        protected virtual void Log(string messageTag, LogType type, object message)
        {
            var logMessage = new LogMessage(DateTime.Now.ToString("yyyy-M-d HH:mm:ss"), messageTag, type,
                message.ToString());
            _localLogger?.Log(logMessage);
            _remoteLogger?.Log(logMessage);
        }

        /// <summary>
        /// Logs a data message.
        /// </summary>
        /// <param name="message">The data message to log.</param>
        public void LogData(string message)
        {
            var logMessage = new LogMessage(message);
            _localLogger?.Log(logMessage);
            _remoteLogger?.Log(logMessage);
        }

        /// <summary>
        /// Stops logging asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StopAsync()
        {
            if (_localLogger != null)
                await _localLogger.StopAsync();

            if (_remoteLogger != null)
                await _remoteLogger.StopAsync();
        }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="messageTag">The tag associated with the message.</param>
        /// <param name="message">The message to log.</param>
        public void I(string messageTag, object message) => Log(messageTag, LogType.Info, message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="messageTag">The tag associated with the message.</param>
        /// <param name="message">The message to log.</param>
        public void W(string messageTag, object message) => Log(messageTag, LogType.Warning, message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="messageTag">The tag associated with the message.</param>
        /// <param name="message">The message to log.</param>
        public void E(string messageTag, object message) => Log(messageTag, LogType.Error, message);
    }
}
