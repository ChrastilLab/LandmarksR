using LandmarksR.Scripts.Experiment.Log;

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
