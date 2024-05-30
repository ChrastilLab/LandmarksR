using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LandmarksR.Scripts.Experiment.Log
{
    /// <summary>
    /// Manages logging for the experiment, including general logging and data logging.
    /// </summary>
    public class ExperimentLogger : MonoBehaviour
    {
        public static ExperimentLogger Instance { get; private set; }

        [SerializeField] private List<string> tagToPrint = new() { "default" };

        private bool CheckTag(string messageTag) => tagToPrint.IndexOf(messageTag) >= 0;

        private TextLogger _generalLogger;
        private Settings _settings;
        private Dictionary<string, DataLogger> _dataLoggers = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance.gameObject);
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
            _generalLogger = new TextLogger();

            if (_settings.logging.localLogging)
            {
                _generalLogger.EnableLocalLog(GetPersistentLocalPath());
            }

            if (_settings.logging.remoteLogging)
            {
                _generalLogger.EnableRemoteLog(GetRelativeRemotePath(),
                    _settings.logging.remoteStatusUrl, _settings.logging.remoteLogUrl);
            }
        }

        public void BeginDataset(string setName, List<string> columnNames)
        {
            var dataLogger = new DataLogger();
            var fileName = $"{setName}.{_settings.logging.dataFileExtension}";

            if (_settings.logging.localLogging)
            {
                dataLogger.EnableLocalLog(GetPersistentLocalPath(fileName));
            }

            if (_settings.logging.remoteLogging)
            {
                dataLogger.EnableRemoteLog(GetRelativeRemotePath(fileName),
                    _settings.logging.remoteStatusUrl, _settings.logging.remoteLogUrl);
            }

            I("output", "BeginDataRow:" + fileName);
            I("output", "Columns:" + string.Join(",", columnNames));

            dataLogger.Begin(columnNames, _settings.logging.dataFileDelimiter);

            _dataLoggers.Add(setName, dataLogger);
        }

        public void SetData(string setName, string column, string value)
        {
            if (!_dataLoggers.ContainsKey(setName))
            {
                E("output", $"Data logger for {setName} not found.");
                return;
            }

            I("output", $"LogData:{column}:{value}");

            _dataLoggers[setName].SetValue(column, value);
        }

        public void LogDataRow(string setName)
        {
            if (!_dataLoggers.ContainsKey(setName))
            {
                E("output", $"Data logger for {setName} not found.");
                return;
            }

            I("output", "LogDataRow:" + setName);

            _dataLoggers[setName].Log();
        }

        public void EndDataset(string setName)
        {
            if (!_dataLoggers.ContainsKey(setName))
            {
                E("output", $"Data logger for {setName} not found.");
                return;
            }

            I("output", "EndDataRow:" + setName);
            _dataLoggers[setName].End();
            _dataLoggers.Remove(setName);
        }

        public void I(string messageTag, object message)
        {
            _generalLogger.I(messageTag, message);
#if UNITY_EDITOR
            if (CheckTag(messageTag))
                Debug.Log($"[LMR] <color=green>INFO</color> | {messageTag} | {message}");
#endif
        }

        public void W(string messageTag, object message)
        {
            _generalLogger.W(messageTag, message);
#if UNITY_EDITOR
            Debug.LogWarning($"[LMR] <color=yellow>WARNING</color> | {messageTag} | {message}");
#endif
        }

        public void E(string messageTag, object message)
        {
            _generalLogger.E(messageTag, message);
#if UNITY_EDITOR
            Debug.LogError($"[LMR] <color=red>ERROR</color> | {messageTag} | {message}");
            EditorApplication.isPlaying = false;
#endif
        }

        private string GetPersistentLocalPath(string fileName = "all.log")
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return Path.Combine(Application.persistentDataPath,
                Application.productName,
                _settings.experiment.participantId,
                $"{_settings.experiment.participantId}_{date}_{fileName}");
        }

        private string GetRelativeRemotePath(string fileName = "all.log")
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return
                $"{Application.productName}/{_settings.experiment.participantId}/{_settings.experiment.participantId}_{date}_{fileName}";
        }

        private async void OnDisable()
        {
            await _generalLogger.StopAsync();
            foreach (var dataLogger in _dataLoggers.Values.ToList())
            {
                await dataLogger.StopAsync();
            }
        }
    }
}
