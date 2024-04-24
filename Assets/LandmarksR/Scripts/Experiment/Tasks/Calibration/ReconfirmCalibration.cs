namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ReconfirmCalibration: InstructionTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            base.Prepare();
            UnregisterConfirmHandler(); // Unregister the confirm handler from the parent class, because we want to redefine it here

            _parentTask = GetComponentInParent<CalibrateTask>();
            if (_parentTask == null)
            {
                Logger.E("calibration",
                    "ReconfirmCalibrationTask is not a child of CalibrateTask. Please make sure ReconfirmCalibrationTask is a child of CalibrateTask.");
                return;
            }


            // Register Event Handlers
            PlayerEvent.RegisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            PlayerEvent.RegisterTimedVRInputHandler(OVRInput.Button.One, Settings.ui.calibrationTriggerTime, HandleAButton, UpdateProgressBarForAButton);

            HUD.ShowProgressBar();

            Settings.space.ApplyToEnvironment();
        }

        protected override void Finish()
        {
            base.Finish();
            HUD.HideProgressBar();

            PlayerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            PlayerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.One, Settings.ui.calibrationTriggerTime, HandleAButton, UpdateProgressBarForAButton);
        }

        private void HandleIndexTrigger()
        {
            isRunning = false;
        }

        private void HandleAButton()
        {
            _parentTask.ResetAll();
            isRunning = false;
        }

        private void UpdateProgressBarForTrigger(float time)
        {
            HUD.SetProgress(time / Settings.ui.calibrationTriggerTime);
        }
        private void UpdateProgressBarForAButton(float time)
        {
            HUD.SetProgress(time / Settings.ui.calibrationTriggerTime);
        }
    }
}
