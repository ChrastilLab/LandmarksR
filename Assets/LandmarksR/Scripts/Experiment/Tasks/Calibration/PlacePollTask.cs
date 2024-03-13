using System;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public enum PolePosition
    {
        LeftTop,
        RightTop,
        RightBottom,
        LeftBottom
    }

    public class PlacePollTask : BaseTask
    {
        private Settings _settings;
        private Hud _hud;
        private PlayerController _playerController;
        private PlayerEventController _playerEvent;
        private GameObject _rightHandAnchor;

        [Header("Pole Settings")] [SerializeField]
        private GameObject polePrefab;

        [SerializeField] private PolePosition polePosition;
        [NotEditable, SerializeField] private GameObject pole;

        private CollectionTask _parentTask;

        protected override void Prepare()
        {
            if (polePrefab == null)
            {
                UnityEngine.Debug.LogError("Pole prefab is not set. Please set the pole prefab in the inspector.");
                return;
            }

            _parentTask = GetComponentInParent<CollectionTask>();
            if (_parentTask == null)
            {
                UnityEngine.Debug.LogError(
                    "PlacePollTask is not a child of CollectionTask. Please make sure PlacePollTask is a child of CollectionTask.");
                return;
            }

            base.Prepare();

            // Assigning references
            _settings = Settings.Instance;
            _hud = Experiment.Instance.playerController.hud;
            _playerController = Experiment.Instance.playerController;
            _playerEvent = _playerController.playerEvent;
            _rightHandAnchor = _playerController.vrPlayerControllerReference.rightHandAnchor;

            // Register Event Handlers
            _playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            _playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            // Show Instruction
            _hud.SetTitle("Calibration")
                .SetContent($"Please place the pole at the {PolePositionToString()} corner of the space and press the trigger button." +
                            "If you want to go back to the previous task, press the A button.")
                .ShowAll();

            // Handle Existing Pole
            if (pole)
            {
                Destroy(pole);
            }
        }

        protected override void Finish()
        {
            base.Finish();
            _playerEvent.UnregisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, HandleIndexTrigger);
            _playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            switch (polePosition)
            {
                case PolePosition.LeftTop:
                    _settings.space.leftTop = _rightHandAnchor.transform.position;
                    break;
                case PolePosition.RightTop:
                    _settings.space.rightTop = _rightHandAnchor.transform.position;
                    break;
                case PolePosition.RightBottom:
                    _settings.space.rightBottom = _rightHandAnchor.transform.position;
                    break;
                case PolePosition.LeftBottom:
                    _settings.space.leftBottom = _rightHandAnchor.transform.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleIndexTrigger()
        {
            var position = _rightHandAnchor.transform.position;
            position.y = 0;
            pole = Instantiate(polePrefab, position, Quaternion.identity);

            isRunning = false;
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();

            isRunning = false;
        }

        private string PolePositionToString() => polePosition switch
        {
            PolePosition.LeftTop => "Left Top",
            PolePosition.RightTop => "Right Top",
            PolePosition.RightBottom => "Right Bottom",
            PolePosition.LeftBottom => "Left Bottom",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
