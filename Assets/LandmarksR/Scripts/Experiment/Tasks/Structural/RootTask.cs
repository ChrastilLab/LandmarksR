using System.Collections.Generic; // Add this for List<T>
using UnityEngine;
using UnityEngine.SceneManagement;
using LandmarksR.Scripts.Experiment.Data; // Assuming this is where TextTable is located

namespace LandmarksR.Scripts.Experiment.Tasks.Structural
{
    public class RootTask : BaseTask
    {
        [SerializeField]
        private GameObject dataHolder;  // Reference to the GameObject that holds the TextTable

        [SerializeField]
        private List<int> scenesToSkip; // List to define which scenes to skip

        private TextTable textTable;

        protected override void Prepare()
        {
            SetTaskType(TaskType.Structural);
            base.Prepare();
            Logger.I("app", "Start Application");

            // Get the TextTable component from the dataHolder GameObject
            textTable = dataHolder.GetComponent<TextTable>();

            if (textTable == null)
            {
                Logger.E("app", "TextTable component is missing on dataHolder!");
            }
            else
            {
                Logger.I("app", "TextTable component found and ready.");
            }
        }

        public override void Finish()
        {
            // Get the current scene index
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Check if the current scene index is in the list of scenes to skip
            if (scenesToSkip.Contains(currentSceneIndex))
            {
                Logger.I("app", $"Skipping scene {currentSceneIndex} as defined in the Inspector.");
                LoadNextScene();  // Skip the current scene and load the next one
                return;
            }

            // Check if textTable and its rows are properly loaded
            if (textTable != null && textTable.StringRows != null && textTable.StringRows.Count > currentSceneIndex)
            {
                // Fetch the current row for the scene
                string currentRow = textTable.StringRows[currentSceneIndex];
                Logger.I("app", $"Processing scene {currentSceneIndex}. Current row: {currentRow}");

                // Check if the row can be split into at least 3 parts (Type, Arg1, Arg2)
                string[] rowParts = currentRow.Split(new[] { ',', '\t' }, System.StringSplitOptions.None);
                Logger.I("app", $"Row split into {rowParts.Length} parts.");

                if (rowParts.Length >= 3)
                {
                    string type = rowParts[0].Trim();
                    string arg1 = rowParts[1].Trim();
                    string arg2 = rowParts[2].Trim();

                    // Log the parts we are checking
                    Logger.I("app", $"Checking row parts - Type: {type}, Arg1: {arg1}, Arg2: {arg2}");

                    // Check if the row matches "Permute Wall", "CDAB", and an empty Arg2
                    if (type == "Permute Wall" && arg1 == "CDAB" && string.IsNullOrEmpty(arg2))
                    {
                        Logger.I("app", "Skipping scene with row value: Permute Wall, CDAB, (empty Arg2)");
                        LoadNextScene();  // Skip the current scene and load the next one
                        return;
                    }
                    else
                    {
                        Logger.I("app", "This row does not match the skip condition.");
                    }
                }
                else
                {
                    Logger.W("app", $"Row format is incorrect or missing parts. Row: {currentRow}");
                }
            }
            else
            {
                Logger.E("app", "TextTable rows are not properly loaded or missing data.");
            }

            // Log that the scene is not being skipped and is proceeding as normal
            Logger.I("app", "No need to skip this scene. Proceeding to load the next scene.");
            LoadNextScene();
        }

        private void LoadNextScene()
        {
            // Calculate the next scene index
            var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            // Check if the next scene is available
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Logger.I("app", "Loading next scene.");
                SceneManager.LoadScene(nextSceneIndex);  // Load the next scene
            }
            else
            {
                Logger.I("app", "No more scenes available. Ending application.");

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();  // Exit the application if in build
#endif
            }
        }
    }
}
