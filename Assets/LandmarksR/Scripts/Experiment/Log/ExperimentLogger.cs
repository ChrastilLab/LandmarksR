using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace LandmarksR.Scripts.Experiment.Log
{
    public class ExperimentLogger : MonoBehaviour
    {
        public static ExperimentLogger Instance { get; private set; }
        [SerializeField] private List<string> tagToPrint = new() { "default" };
        private bool CheckTag(string messageTag) => tagToPrint.IndexOf(messageTag) >= 0;

        private TextLogger _textLogger;
        private Settings _settings;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }

            Init();
            DontDestroyOnLoad(this);
        }

        private void Init()
        {
            _settings = Settings.Instance;
            _textLogger = new TextLogger();

            if (_settings.logging.localLogging)
            {
                _textLogger.EnableLocalLog(GetPersistentLocalPath($"{_settings.experiment.participantId}.log"));
            }

            if (_settings.logging.remoteLogging)
            {
                _textLogger.EnableRemoteLog(GetRelativeRemotePath($"{_settings.experiment.participantId}.log"),
                    _settings.logging.remoteStatusUrl, _settings.logging.remoteLogUrl);
            }
        }

        public void I(string messageTag, string message)
        {
            if (!CheckTag(messageTag)) return;
#if UNITY_EDITOR
            Debug.Log($"<color=green>INFO</color> | {messageTag} | {message}");
#endif
            _textLogger.I(messageTag, message);
        }

        public void W(string messageTag, string message)
        {
            if (!CheckTag(messageTag)) return;
#if UNITY_EDITOR
            Debug.Log($"<color=yellow>WARNING</color> | {messageTag} | {message}");
#endif
            _textLogger.W(messageTag, message);
        }

        public void E(string messageTag, string message)
        {
            if (!CheckTag(messageTag)) return;
#if UNITY_EDITOR
            Debug.Log($"<color=red>ERROR</color> | {messageTag} | {message}");
#endif
            _textLogger.E(messageTag, message);
        }

        private string GetPersistentLocalPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath,
                Application.productName,
                _settings.experiment.participantId,
                fileName);
        }

        private string GetRelativeRemotePath(string fileName)
        {
            return $"{Application.productName}/{_settings.experiment.participantId}/{fileName}";
        }

        private async void OnDisable()
        {
            await _textLogger.StopAsync();
        }
    }
}
