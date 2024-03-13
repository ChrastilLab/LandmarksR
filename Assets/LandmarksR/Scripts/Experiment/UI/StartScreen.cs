using System.Collections.Generic;
using System.IO;
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
        [SerializeField] private HorizontalSelection displayModeSelection;
        [SerializeField] private TMP_Text errorText;

        private readonly List<string> _displayModeOptions = new() { "VR", "PC" };

        public void Start()
        {
            var scenes = GetAllScenes();
            sceneModeSelection.SetList(scenes);

            displayModeSelection.SetList(_displayModeOptions);
        }
        public void StartExperiment()
        {
            if (!ValidateNonNullity()) return;
            if (!ValidateFields()) return;
            SceneManager.LoadScene(sceneModeSelection.GetSelectedOption());
        }

        private static IEnumerable<string> GetAllScenes()
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

            if (displayModeSelection == null)
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

            Settings.Instance.experiment.participantId = participantIdInput.text;
            Settings.Instance.displayReference.displayMode = displayModeSelection.GetSelectedOption() switch
            {
                "PC" => DisplayMode.Desktop,
                "VR" => DisplayMode.VR,
                _ => throw new System.Exception("Invalid Display Mode, Check The HorizontalSelection Options")
            };

            return true;
        }
    }
}
