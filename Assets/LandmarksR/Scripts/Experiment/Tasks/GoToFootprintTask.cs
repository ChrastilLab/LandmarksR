using LandmarksR.Scripts.Attributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    /// <summary>
    /// Represents a task where the player needs to go to a specified footprint.
    /// </summary>
    public class GoToFootprintTask : BaseTask
    {
        /// <summary>
        /// The target transform the player needs to reach.
        /// </summary>
        [NotEditable, SerializeField] private Transform target;

        /// <summary>
        /// The footprint prefab to instantiate.
        /// </summary>
        [SerializeField] private GameObject footprintPrefab;

        /// <summary>
        /// The instantiated footprint component.
        /// </summary>
        private Footprint _footprint;

        /// <summary>
        /// Indicates whether to move to the origin.
        /// </summary>
        [SerializeField] private bool toOrigin;

        /// <summary>
        /// Offset from the origin.
        /// </summary>
        [SerializeField] private Vector3 originOffset;

        /// <summary>
        /// Indicates whether the player is on the footprint.
        /// </summary>
        private bool _isPlayerOnFootprint;

        /// <summary>
        /// Indicates whether the player is ready to confirm their position.
        /// </summary>
        private bool _readyToConfirm;

        /// <summary>
        /// The environment GameObject.
        /// </summary>
        private GameObject _environment;

        /// <summary>
        /// Hides the environment except for the floor.
        /// </summary>
        private void HideEnvironment()
        {
            foreach (Transform child in _environment.transform)
            {
                if (child.CompareTag("Floor")) continue;
                child.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Shows the environment.
        /// </summary>
        private void ShowEnvironment()
        {
            foreach (Transform child in _environment.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Prepares the go to footprint task.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();

            // Disable environment
            _environment = GameObject.FindGameObjectWithTag("Environment");
            Assert.IsNotNull(_environment, "Environment GameObject not found. Please tag the environment with 'Environment'");

            HideEnvironment();

            if (toOrigin)
            {
                target = new GameObject("Origin").transform;

                var position = Settings.space.calibrated ? Settings.space.center : Vector3.zero;
                position += _environment.transform.rotation * originOffset;

                Logger.I("calibration", $"origin position: {originOffset}");
                Logger.I("calibration", $"Target position: {position}");

                var rotation = Settings.space.calibrated ? Quaternion.LookRotation(Settings.space.forward) : Quaternion.identity;
                target.transform.SetPositionAndRotation(position, rotation);
            }

            var footprintGameObject = Instantiate(footprintPrefab, target.position, target.rotation);
            _footprint = footprintGameObject.GetComponent<Footprint>();

            Assert.IsNotNull(_footprint, "Footprint prefab must have a Footprint component.");

            _footprint.TriggerEnterAction += HandlePlayerTriggerEnter;
            _footprint.TriggerExitAction += HandlePlayerTriggerExit;

            HUD.SetTitle("")
                .SetContent($"Please look for a footprint and step on it.")
                .ShowAll()
                .HideButton();

            Player.TryEnableDesktopInput(3f);
            PlayerEvent.RegisterConfirmHandler(HandleConfirm);
        }

        /// <summary>
        /// Handles the confirm event when the player confirms their position.
        /// </summary>
        private void HandleConfirm()
        {
            if (!_readyToConfirm) return;

            StopCurrentTask();
        }

        /// <summary>
        /// Handles the event when the player enters the footprint trigger.
        /// </summary>
        /// <param name="other">The collider of the object the player triggered.</param>
        private void HandlePlayerTriggerEnter(Collider other)
        {
            var obj = other.transform;
            if (!obj.CompareTag("PlayerCollider")) return;

            _isPlayerOnFootprint = true;

            HUD.FixedRecenter(2f);
            HUD.SetContent("Please align your foot with the footprint.")
                .ShowAll();
        }

        /// <summary>
        /// Handles the event when the player exits the footprint trigger.
        /// </summary>
        /// <param name="other">The collider of the object the player triggered.</param>
        private void HandlePlayerTriggerExit(Collider other)
        {
            var obj = other.transform;
            if (!obj.CompareTag("PlayerCollider")) return;

            _isPlayerOnFootprint = false;
            _readyToConfirm = false;

            HUD.HideAll();
        }

        /// <summary>
        /// Updates the task, checking the player's alignment with the footprint.
        /// </summary>
        private void Update()
        {
            if (!IsTaskRunning()) return;
            if (!_isPlayerOnFootprint) return;

            HUD.FixedRecenter(2f);
            var angleDifference = ComputeAngleDifference();
            switch (angleDifference)
            {
                case < 10 and > -10:
                    HUD.SetContent("Aligned! Press Trigger to continue.");
                    _readyToConfirm = true;
                    break;
                case < -10:
                    HUD.SetContent("You are not aligned. Slowly rotate to your left to align.");
                    _readyToConfirm = false;
                    break;
                default:
                    HUD.SetContent("You are not aligned. Slowly rotate to your right to align.");
                    _readyToConfirm = false;
                    break;
            }
        }

        /// <summary>
        /// Finishes the task, showing the environment and clearing the HUD.
        /// </summary>
        public override void Finish()
        {
            base.Finish();
            ShowEnvironment();
            Player.DisableDesktopInput();
            PlayerEvent.UnregisterConfirmHandler(HandleConfirm);

            if (_footprint)
                Destroy(_footprint.gameObject);
        }

        /// <summary>
        /// Computes the angle difference between the player's forward direction and the target's forward direction.
        /// </summary>
        /// <returns>The angle difference in degrees.</returns>
        private float ComputeAngleDifference()
        {
            var playerForward = Player.GetMainCamera().transform.forward;
            var targetForward = _footprint.transform.forward;

            return Vector3.SignedAngle(playerForward, targetForward, Vector3.up);
        }
    }
}
