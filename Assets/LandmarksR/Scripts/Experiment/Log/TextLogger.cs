using System;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class TextLogger : MonoBehaviour
    {
        [SerializeField] private bool enableLocalLog = true;
        [SerializeField] private string localLogPath = "log.txt";

        [SerializeField] private bool enableRemoteLog;
        [SerializeField] private string remoteLogUrl = "http://localhost:3000/log";
        [SerializeField] private string remoteStatusUrl = "http://localhost:3000/status";
        [SerializeField] private string remoteFilePath = "log.txt";

        private LocalLogger _localLogger;
        private RemoteLogger _remoteLogger;

        protected virtual void Awake()
        {
            if (enableLocalLog)
            {
                _localLogger = new LocalLogger(localLogPath);
            }

            if (enableRemoteLog)
            {
                _remoteLogger = new RemoteLogger(remoteStatusUrl, remoteLogUrl);
            }
        }

        protected virtual void Log(string messageTag, LogPriority priority, string message)
        {
            var logMessage = new LogMessage( DateTime.Now.ToString("yyyy-M-d HH:mm:ss"), messageTag, priority, message);
            if (enableLocalLog)
            {
                _localLogger.Log(logMessage);
            }

            if (enableRemoteLog)
            {
                logMessage.FileName = remoteFilePath;
                _remoteLogger.Log(logMessage);
            }
        }

        private async void OnDisable()
        {
            if (enableLocalLog)
            {
                await _localLogger.StopAsync();
            }

            if (enableRemoteLog)
            {
                await _remoteLogger.StopAsync();
            }
        }

        public virtual void I(string messageTag, string message) => Log(messageTag, LogPriority.Info, message);
        public virtual void W(string messageTag, string message) => Log(messageTag, LogPriority.Warning, message);
        public virtual void E(string messageTag, string message) => Log(messageTag, LogPriority.Error, message);
    }
}
