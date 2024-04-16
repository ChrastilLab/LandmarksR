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

            playerEvent.RegisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBar);
            playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            hud.ShowProgressBar();

            _parentTask.InitializeFloorIndicator();
        }

        protected override void Finish()
        {
            base.Finish();
            hud.HideProgressBar();

            playerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBar);
            playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);

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
            hud.SetProgress(time / settings.ui.calibrationTriggerTime);
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
