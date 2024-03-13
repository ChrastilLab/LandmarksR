using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class RootTask : BaseTask
    {
        protected override void Prepare()
        {
            ExperimentLogger.Instance.I("app", "Start Application");
            base.Prepare();
        }
        protected override void Finish()
        {
            ExperimentLogger.Instance.I("app", "Finish Application");
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
