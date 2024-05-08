using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class TextLogger
    {

        // private string remoteLogUrl = "http://localhost:3000/log";
        // private string remoteStatusUrl = "http://localhost:3000/status";

        protected LocalLogger _localLogger;
        protected RemoteLogger _remoteLogger;


        public void EnableLocalLog(string filePath = "log.txt")
        {

            _localLogger = new LocalLogger(filePath);
        }
        public void EnableRemoteLog(string filePath = "log.txt", string remoteStatusUrl = "http://localhost:3000/status", string remoteLogUrl = "http://localhost:3000/log")
        {
            _remoteLogger = new RemoteLogger(filePath, remoteStatusUrl, remoteLogUrl);
        }

        protected virtual void Log(string messageTag, LogType type, object message)
        {
            var logMessage = new LogMessage( DateTime.Now.ToString("yyyy-M-d HH:mm:ss"), messageTag, type, message.ToString());
            _localLogger?.Log(logMessage);
            _remoteLogger?.Log(logMessage);
        }

        public void LogData(string message)
        {
            _localLogger?.Log(new LogMessage(message));
            _remoteLogger?.Log(new LogMessage(message));
        }

        public async Task StopAsync()
        {
            await _localLogger?.StopAsync()!;
            await _remoteLogger?.StopAsync()!;
        }

        public void I(string messageTag, object message) => Log(messageTag, LogType.Info, message);
        public void W(string messageTag, object message) => Log(messageTag, LogType.Warning, message);
        public void E(string messageTag, object message) => Log(messageTag, LogType.Error, message);

        private string GetFileName()
        {
            // CompanyName
            return "";
        }
    }
}
