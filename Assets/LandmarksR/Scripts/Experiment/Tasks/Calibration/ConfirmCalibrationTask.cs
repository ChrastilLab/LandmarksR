using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Calibration
{
    public class ConfirmCalibrationTask : InstructionTask
    {
        private Settings _settings;
        private PlayerEventController _playerEvent;
        private CollectionTask _parentTask;

        [SerializeField] private GameObject calibrationResultPrefab;
        private GameObject _calibrationResult;
        protected override void Prepare()
        {
            // Check references
            _parentTask = GetComponentInParent<CollectionTask>();
            if (_parentTask == null)
            {
                UnityEngine.Debug.LogError(
                    "ConfirmCalibrationTask is not a child of CollectionTask. Please make sure ConfirmCalibrationTask is a child of CollectionTask.");
                return;
            }

            base.Prepare();

            // Assigning references
            _settings = Settings.Instance;
            _playerEvent = Experiment.Instance.playerController.playerEvent;

            // Register Event Handlers
            _playerEvent.RegisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            ComputeCalibration();

        }


        protected override void Finish()
        {
            base.Finish();
            _playerEvent.UnregisterVRInputHandler(OVRInput.Button.One, HandleAButton);

            if (_calibrationResult)
            {
                Destroy(_calibrationResult);
                _settings.space.ApplyToEnvironment();
            }
        }

        private void HandleAButton()
        {
            _parentTask.MoveToPrevious();
            isRunning = false;
        }

        private void ComputeCalibration()
        {
            var center = _settings.space.ComputeCenter();
            var forward = _settings.space.ComputeForward();

            logger.I("calibration", $"Center: {center}|Forward: {forward}");

            _calibrationResult = Instantiate(calibrationResultPrefab, center, Quaternion.LookRotation(forward));
        }
    }
}
