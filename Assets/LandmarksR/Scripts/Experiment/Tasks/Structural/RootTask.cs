﻿namespace LandmarksR.Scripts.Experiment.Tasks.Structural
{
    public class RootTask : BaseTask
    {
        protected override void Prepare()
        {
            SetTaskType(TaskType.Structural);
            base.Prepare();
            Logger.I("app", "Start Application");
        }
        public override void Finish()
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
    }
}
