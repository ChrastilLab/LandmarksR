using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class CalibrateFloorTask : BaseTask
    {

        private Settings _settings;
        private Hud _hud;
        private PlayerController _playerController;
        private PlayerEventController _playerEvent;
        private GameObject _rightHandAnchor;
        private CollectionTask _parentTask;
        protected override void Prepare()
        {
            _parentTask = GetComponentInParent<CollectionTask>();
            base.Prepare();
            _settings = Settings.Instance;
            _hud = Experiment.Instance.playerController.hud;
            _playerController = Experiment.Instance.playerController;
            _playerEvent = _playerController.playerEvent;
            _rightHandAnchor = _playerController.vrPlayerControllerReference.rightHandAnchor;

            _playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            _playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            _hud.SetTitle("Calibrate Floor")
                .SetContent(
                    "Please place the controller on the floor and press the trigger button to set the floor position.")
                .ShowAll();
        }

        protected override void Finish()
        {
            base.Finish();
            _playerEvent.UnregisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            _playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);
        }

        private void HandleIndexTrigger()
        {
            // Set the floor position
            _settings.space.groundY = _rightHandAnchor.transform.position.y;
            isRunning = false;
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();
            isRunning = false;
        }
    }
}
