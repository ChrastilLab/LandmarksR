namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class CollectionTask : BaseTask
    {
        protected override void Prepare()
        {
            base.Prepare();
            isRunning = false;
        }
    }
}
