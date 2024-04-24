namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ApplyCalibrationTask : BaseTask
    {
        protected override void Prepare()
        {
            base.Prepare();

            if (Settings.space.calibrated)
            {
                Settings.space.ApplyToEnvironment();
            }
            else
            {
                Logger.W("task", "Calibration not found.");
            }

            isRunning = false;
        }
    }
}
