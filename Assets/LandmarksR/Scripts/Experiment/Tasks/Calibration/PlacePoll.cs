namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class PlacePoll : BaseTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            base.Prepare();

            _parentTask = GetComponentInParent<CalibrateTask>();
            if (_parentTask == null)
            {
                logger.E("calibration",
                    "PlacePollTask is not a child of CollectionTask. Please make sure PlacePollTask is a child of CollectionTask.");
                return;
            }

            // Register Event Handlers
            playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            // Show Instruction
            hud.SetTitle("Calibration")
                .SetContent(
                    $"Please place the pole at the {_parentTask.GetCurrentPolePosition()} corner of the space and press the trigger button." +
                    "If you want to go back to the previous task, press the A button.")
                .ShowAll();
        }

        protected override void Finish()
        {
            base.Finish();
            playerEvent.UnregisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);
        }

        private void HandleIndexTrigger()
        {
            _parentTask.AddPole();
            isRunning = false;
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();
            _parentTask.RemoveLastPole();
            isRunning = false;
        }
    }
}
