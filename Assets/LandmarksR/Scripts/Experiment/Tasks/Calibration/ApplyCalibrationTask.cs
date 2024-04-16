namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ApplyCalibrationTask : BaseTask
    {
        protected override void Prepare()
        {
            base.Prepare();

            if (settings.space.calibrated)
            {
                settings.space.ApplyToEnvironment();
            }
            else
            {
                logger.W("task", "Calibration not found.");
            }

            isRunning = false;
        }
    }
}
