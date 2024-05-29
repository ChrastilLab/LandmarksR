namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class PlacePoll : BaseTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();

            _parentTask = GetComponentInParent<CalibrateTask>();
            if (_parentTask == null)
            {
                Logger.E("calibration",
                    "PlacePollTask is not a child of CollectionTask. Please make sure PlacePollTask is a child of CollectionTask.");
                return;
            }

            HUD.FixedRecenter(2f);

            // Register Event Handlers
            PlayerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            PlayerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            // Show Instruction
            HUD.SetTitle("Calibration")
                .SetContent(
                    $"Please place the pole at the {_parentTask.GetCurrentPolePosition()} corner of the space and press the trigger button." +
                    "If you want to go back to the previous task, press the A button.")
                .ShowAll();
        }

        public override void Finish()
        {
            base.Finish();
            PlayerEvent.UnregisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            PlayerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);
        }

        private void HandleIndexTrigger()
        {
            _parentTask.AddPole();
            StopCurrentTask();
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();
            _parentTask.RemoveLastPole();
            StopCurrentTask();
        }
    }
}
