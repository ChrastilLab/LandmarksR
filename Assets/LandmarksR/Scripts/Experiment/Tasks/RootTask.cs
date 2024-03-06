using LandmarksR.Scripts.Utility;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class RootTask : BaseTask
    {
        protected override void Prepare()
        {
            DebugLogger.Instance.Log("Start Application", "app");
        }
        protected override void Finish()
        {
            DebugLogger.Instance.Log("Quit Application", "app");
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
