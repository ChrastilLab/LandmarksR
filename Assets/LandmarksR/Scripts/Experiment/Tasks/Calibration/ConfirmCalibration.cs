namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ConfirmCalibration : InstructionTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            base.Prepare();
            // Check references
            _parentTask = GetComponentInParent<CalibrateTask>();
            if (_parentTask == null)
            {
                logger.E("calibration",
                    "ConfirmCalibrationTask is not a child of CalibrateTask. Please make sure ConfirmCalibrationTask is a child of CalibrateTask.");
                return;
            }

            // Register Event Handlers
            playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            ComputeCalibration();
        }


        protected override void Finish()
        {
            base.Finish();
            playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);
        }

        private void HandleAButton()
        {
            _parentTask.RemoveCalibrationResultIndicator();
            _parentTask.MoveToPrevious();
            isRunning = false;
        }

        private void ComputeCalibration()
        {
            _parentTask.UpdateFloorPositionInSettings();
            _parentTask.UpdatePolePositionsInSettings(); // Update the pole positions in the settings
            settings.space.CalibrateSpace(); // Calibrate the space based on the pole positions
            _parentTask.ShowCalibrationResultIndicator(settings.space.center, settings.space.forward);
        }
    }
}
