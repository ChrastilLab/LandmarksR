using System.Collections.Generic;
using System.IO;
using System.Linq;
using LandmarksR.Scripts.Player;
using LandmarksR.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LandmarksR.Scripts.Experiment.UI
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TMP_InputField participantIdInput;
        [SerializeField] private HorizontalSelection sceneModeSelection;
        [SerializeField] private HorizontalSelection settingsSelection;
        [SerializeField] private TMP_Text errorText;

        private List<Settings> _availableSettings;

        public void Start()
        {
            var scenes = GetActiveScenes();
            sceneModeSelection.SetList(scenes);
            settingsSelection.SetList(GetAvailableSettings());
        }
        public void StartExperiment()
        {
            if (!ValidateNonNullity()) return;
            if (!ValidateFields()) return;
            SceneManager.LoadScene(sceneModeSelection.GetSelectedOption());
        }

        private static IEnumerable<string> GetActiveScenes()
        {
            var scenes = new List<string>();
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                if (sceneName != SceneManager.GetActiveScene().name)
                    scenes.Add(sceneName);
            }

            return scenes;
        }

        private IEnumerable<string> GetAvailableSettings()
        {
            // Get all available settings from resources folder

            _availableSettings = Resources.LoadAll<Settings>("Settings").ToList();

            return _availableSettings.Select(settings => settings.name).ToList();

        }

        private bool ValidateNonNullity()
        {
            if (participantIdInput == null)
            {
                Debug.LogError("Participant ID Input has not been assigned to the StartScreen script. This is a critical error");
                return false;
            }

            if (sceneModeSelection == null)
            {
                Debug.LogError(
                    "Scene Mode Selection has not been assigned to the StartScreen script. This is a critical error");
                return false;
            }

            if (settingsSelection == null)
            {
                Debug.LogError(
                    "Display Mode Selection has not been assigned to the StartScreen script. This is a critical error");
                return false;
            }

            if (errorText == null)
            {
                Debug.LogError(
                    "Error Text has not been assigned to the StartScreen script. This is not a critical error, but it is recommended to assign one");
                return false;
            }

            return true;
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(participantIdInput.text))
            {
                errorText.text = "Please enter a participant ID";
                return false;
            }

            // Settings.Instance.experiment.participantId = participantIdInput.text;

            if (sceneModeSelection.GetSelectedOption() == null)
            {
                errorText.text = "Please select a scene";
                return false;
            }

            var settings = _availableSettings[settingsSelection.GetSelectedIndex()];
            settings.experiment.participantId = participantIdInput.text;

            // Create a new instance of the selected settings
            var settingClone = Instantiate(settings);

            settingClone.name = settings.name;

            return true;
        }
    }
}
