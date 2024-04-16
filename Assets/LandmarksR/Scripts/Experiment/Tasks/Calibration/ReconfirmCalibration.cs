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
                logger.E("calibration",
                    "ReconfirmCalibrationTask is not a child of CalibrateTask. Please make sure ReconfirmCalibrationTask is a child of CalibrateTask.");
                return;
            }


            // Register Event Handlers
            playerEvent.RegisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            playerEvent.RegisterTimedVRInputHandler(OVRInput.Button.One, settings.ui.calibrationTriggerTime, HandleAButton, UpdateProgressBarForAButton);

            hud.ShowProgressBar();

            settings.space.ApplyToEnvironment();
        }

        protected override void Finish()
        {
            base.Finish();
            hud.HideProgressBar();

            playerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            playerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.One, settings.ui.calibrationTriggerTime, HandleAButton, UpdateProgressBarForAButton);
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
            hud.SetProgress(time / settings.ui.calibrationTriggerTime);
        }
        private void UpdateProgressBarForAButton(float time)
        {
            hud.SetProgress(time / settings.ui.calibrationTriggerTime);
        }
    }
}
