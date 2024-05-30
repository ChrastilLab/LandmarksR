using LandmarksR.Scripts.Experiment.Tasks.Interactive;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class PlaceFloor : InstructionTask
    {
        private CalibrateTask _parentTask;

        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();
            UnregisterConfirmHandler(); // Unregister the confirm handler from the parent class, because we want to redefine it here

            _parentTask = GetComponentInParent<CalibrateTask>();

            PlayerEvent.RegisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBar);
            PlayerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            HUD.ShowProgressBar();

            _parentTask.InitializeFloorIndicator();
        }

        public override void Finish()
        {
            base.Finish();
            HUD.HideProgressBar();

            PlayerEvent.UnregisterTimedVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Settings.ui.calibrationTriggerTime, HandleIndexTrigger, UpdateProgressBar);
            PlayerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);

        }

        private void HandleIndexTrigger()
        {
            StopCurrentTask();
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();
            _parentTask.RemoveLastPole();
            _parentTask.RemoveFloorIndicator();

            StopCurrentTask();
        }

        private void UpdateProgressBar(float time)
        {
            HUD.SetProgress(time / Settings.ui.calibrationTriggerTime);
        }

        private void Update()
        {
            if (IsTaskRunning())
            {
                _parentTask.UpdateFloorIndicator();
            }
        }
    }
}
