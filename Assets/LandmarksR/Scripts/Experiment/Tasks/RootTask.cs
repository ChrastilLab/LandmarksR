using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class RootTask : BaseTask
    {
        protected override void Prepare()
        {
            logger = ExperimentLogger.Instance;
            logger.I("app", "Start Application");
        }
        protected override void Finish()
        {
            // Get the current scene index

            var currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            var nextSceneIndex = currentSceneIndex + 1;

            // Check if next scene is available
            logger.I("app", $"Next Scene Index: {nextSceneIndex}");
            logger.I("app", $"Total Scenes: {UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings}");

            if (nextSceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
            {
                logger.I("app", "Load Next Scene");
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
                return;
            }

            logger.I("app", "Finish Application");

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
