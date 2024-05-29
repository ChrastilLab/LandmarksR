namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class DummyTask: BaseTask
    {
        private void Update()
        {
            StopCurrentTask();
        }
    }
}
