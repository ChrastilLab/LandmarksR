namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class PlaceFloor : InstructionTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            base.Prepare();
            UnregisterConfirmHandler(); // Unregister the confirm handler from the parent class, because we want to redefine it here

            _parentTask = GetComponentInParent<CalibrateTask>();

            PlayerEvent.RegisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBar);
            PlayerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            HUD.ShowProgressBar();

            _parentTask.InitializeFloorIndicator();
        }

        protected override void Finish()
        {
            base.Finish();
            HUD.HideProgressBar();

            PlayerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBar);
            PlayerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);

        }

        private void HandleIndexTrigger()
        {
            isRunning = false;
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();
            _parentTask.RemoveLastPole();
            _parentTask.RemoveFloorIndicator();
            isRunning = false;
        }

        private void UpdateProgressBar(float time)
        {
            HUD.SetProgress(time / Settings.ui.calibrationTriggerTime);
        }

        private void Update()
        {
            if (isRunning)
            {
                _parentTask.UpdateFloorIndicator();
            }
        }
    }
}
