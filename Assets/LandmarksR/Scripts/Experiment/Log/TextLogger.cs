using System;
using System.Threading.Tasks;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class TextLogger
    {

        // private string remoteLogUrl = "http://localhost:3000/log";
        // private string remoteStatusUrl = "http://localhost:3000/status";

        private LocalLogger _localLogger;
        private RemoteLogger _remoteLogger;

        private bool _enableLocalLog = true;
        private bool _enableRemoteLog = false;

        public void EnableLocalLog(string filePath = "log.txt")
        {

            Debug.Log("Local log writes to: " + filePath);
            _localLogger = new LocalLogger(filePath);
            _enableLocalLog = true;
        }
        public void EnableRemoteLog(string filePath = "log.txt", string remoteStatusUrl = "http://localhost:3000/status", string remoteLogUrl = "http://localhost:3000/log")
        {
            _remoteLogger = new RemoteLogger(filePath, remoteStatusUrl, remoteLogUrl);
            _enableRemoteLog = true;
        }





        protected virtual void Log(string messageTag, LogPriority priority, object message)
        {
            var logMessage = new LogMessage( DateTime.Now.ToString("yyyy-M-d HH:mm:ss"), messageTag, priority, message.ToString());
            if (_enableLocalLog)
            {
                _localLogger.Log(logMessage);
            }

            if (_enableRemoteLog)
            {
                _remoteLogger.Log(logMessage);
            }
        }

        public async Task StopAsync()
        {
            if (_enableLocalLog)
            {
                await _localLogger.StopAsync();
            }

            if (_enableRemoteLog)
            {
                await _remoteLogger.StopAsync();
            }
        }

        public virtual void I(string messageTag, object message) => Log(messageTag, LogPriority.Info, message);
        public virtual void W(string messageTag, object message) => Log(messageTag, LogPriority.Warning, message);
        public virtual void E(string messageTag, object message) => Log(messageTag, LogPriority.Error, message);

        private string GetFileName()
        {
            // CompanyName
            return "";
        }
    }
}
