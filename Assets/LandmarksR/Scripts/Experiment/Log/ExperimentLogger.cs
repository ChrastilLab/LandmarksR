using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
                _textLogger.EnableLocalLog(GetPersistentLocalPath());
            }

            if (_settings.logging.remoteLogging)
            {
                _textLogger.EnableRemoteLog(GetRelativeRemotePath(),
                    _settings.logging.remoteStatusUrl, _settings.logging.remoteLogUrl);
            }
        }

        public void I(string messageTag, string message)
        {
            _textLogger.I(messageTag, message);
#if UNITY_EDITOR
            if (CheckTag(messageTag))
                Debug.Log($"[LMR] <color=green>INFO</color> | {messageTag} | {message}");
#endif
        }

        public void W(string messageTag, string message)
        {
            _textLogger.W(messageTag, message);
#if UNITY_EDITOR
            Debug.LogWarning($"[LMR] <color=yellow>WARNING</color> | {messageTag} | {message}");
#endif
        }

        public void E(string messageTag, string message)
        {
            _textLogger.E(messageTag, message);
#if UNITY_EDITOR
            Debug.LogError($"[LMR] <color=red>ERROR</color> | {messageTag} | {message}");
            EditorApplication.isPlaying = false;
#endif
        }

        private string GetPersistentLocalPath()
        {
            var date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return Path.Combine(Application.persistentDataPath,
                Application.productName,
                _settings.experiment.participantId,
                $"{_settings.experiment.participantId}_{date}.log");
        }

        private string GetRelativeRemotePath()
        {
            var date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return
                $"{Application.productName}/{_settings.experiment.participantId}/{_settings.experiment.participantId}_{date}.log";
        }

        private async void OnDisable()
        {
            await _textLogger.StopAsync();
        }
    }
}
