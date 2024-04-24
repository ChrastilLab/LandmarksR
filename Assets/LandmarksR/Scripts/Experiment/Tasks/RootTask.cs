using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class RootTask : BaseTask
    {
        protected override void Prepare()
        {
            Logger = ExperimentLogger.Instance;
            Logger.I("app", "Start Application");
        }
        protected override void Finish()
        {
            // Get the current scene index

            var currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            var nextSceneIndex = currentSceneIndex + 1;

            // Check if next scene is available
            Logger.I("app", $"Next Scene Index: {nextSceneIndex}");
            Logger.I("app", $"Total Scenes: {UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings}");

            if (nextSceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                Logger.I("app", "Load Next Scene");
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
                return;
            }

            Logger.I("app", "Finish Application");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }


        private void Update()
        {
            isRunning = false;
        }
    }
}
