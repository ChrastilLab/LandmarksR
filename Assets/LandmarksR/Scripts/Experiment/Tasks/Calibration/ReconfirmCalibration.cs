using LandmarksR.Scripts.Player;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ReconfirmCalibration: InstructionTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            base.Prepare();

            _parentTask = GetComponentInParent<CalibrateTask>();
            if (_parentTask == null)
            {
                logger.E("calibration",
                    "ReconfirmCalibrationTask is not a child of CalibrateTask. Please make sure ReconfirmCalibrationTask is a child of CalibrateTask.");
                return;
            }


            // Register Event Handlers
            playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            settings.space.ApplyToEnvironment();


        }

        protected override void Finish()
        {
            base.Finish();
            playerEvent.UnregisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);
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
    }
}
