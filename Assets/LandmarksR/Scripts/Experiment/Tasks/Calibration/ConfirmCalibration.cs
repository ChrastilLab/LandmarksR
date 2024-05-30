using LandmarksR.Scripts.Experiment.Tasks.Interactive;
using OVR.OpenVR;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ConfirmCalibration : InstructionTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();
            UnregisterConfirmHandler(); // Unregister the confirm handler from the parent class, because we want to redefine it here

            // Check references
            _parentTask = GetComponentInParent<CalibrateTask>();
            if (_parentTask == null)
            {
                Logger.E("calibration",
                    "ConfirmCalibrationTask is not a child of CalibrateTask. Please make sure ConfirmCalibrationTask is a child of CalibrateTask.");
                return;
            }

            // Register Event Handlers
            PlayerEvent.RegisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            PlayerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            HUD.ShowProgressBar();

            // Compute Calibration
            _parentTask.ComputeCalibration();
        }


        public override void Finish()
        {
            base.Finish();
            HUD.HideProgressBar();

            PlayerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBarForTrigger);
            PlayerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);
        }


        private void HandleIndexTrigger()
        {
            StopCurrentTask();
        }

        private void HandleAButton()
        {
            _parentTask.RemoveCalibrationResultIndicator();
            _parentTask.MoveToPrevious();

            StopCurrentTask();
        }

        private void UpdateProgressBarForTrigger(float time)
        {
            HUD.SetProgress(time / Settings.ui.calibrationTriggerTime);
        }
    }
}
