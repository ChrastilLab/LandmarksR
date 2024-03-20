using OVR.OpenVR;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ConfirmCalibration : InstructionTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            base.Prepare();
            UnregisterConfirmHandler(); // Unregister the confirm handler from the parent class, because we want to redefine it here

            // Check references
            _parentTask = GetComponentInParent<CalibrateTask>();
            if (_parentTask == null)
            {
                logger.E("calibration",
                    "ConfirmCalibrationTask is not a child of CalibrateTask. Please make sure ConfirmCalibrationTask is a child of CalibrateTask.");
                return;
            }

            // Register Event Handlers
            playerEvent.RegisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            hud.ShowProgressBar();

            // Compute Calibration
            _parentTask.ComputeCalibration();
        }


        protected override void Finish()
        {
            base.Finish();
            hud.HideProgressBar();

            playerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);
        }


        private void HandleIndexTrigger()
        {
            isRunning = false;
        }

        private void HandleAButton()
        {
            _parentTask.RemoveCalibrationResultIndicator();
            _parentTask.MoveToPrevious();
            isRunning = false;
        }

        private void UpdateProgressBarForTrigger(float time)
        {
            logger.I("calibration", "UpdateProgressBarForTrigger: " + time);
            hud.SetProgress(time / settings.ui.calibrationTriggerTime);
        }
    }
}
