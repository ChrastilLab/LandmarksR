using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class PlaceFloor : BaseTask
    {

        private GameObject _rightHandAnchor;
        private CalibrateTask _parentTask;
        protected override void Prepare()
        {
            base.Prepare();
            _parentTask = GetComponentInParent<CalibrateTask>();
            _rightHandAnchor = playerController.vrPlayerControllerReference.rightHandAnchor;

            playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            hud.SetTitle("Calibrate Floor")
                .SetContent(
                    "Please place the controller on the floor and press the trigger button to set the floor position.")
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
            // Set the floor position
            _parentTask.UpdateFloorPositionInSettings();
            isRunning = false;
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();
            _parentTask.RemoveLastPole();
            isRunning = false;
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
