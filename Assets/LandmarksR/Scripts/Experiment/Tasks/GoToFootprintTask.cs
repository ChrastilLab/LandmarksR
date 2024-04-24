using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class GoToFootprintTask : BaseTask
    {
        [NotEditable, SerializeField] private Transform target;
        [SerializeField] private GameObject footprintPrefab;
        private Footprint _footprint;

        [SerializeField] private bool toOrigin;
        [SerializeField] private Vector3 originOffset;
        private bool _isPlayerOnFootprint;
        private bool _readyToConfirm;
        private GameObject _environment;



        private void HideEnvironment()
        {
            foreach (Transform child in _environment.transform)
            {
                if (child.CompareTag("Floor")) continue;
                child.gameObject.SetActive(false);
            }
        }

        private void ShowEnvironment()
        {
            foreach (Transform child in _environment.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        protected override void Prepare()
        {
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

            _footprint.onTriggerEnter += HandlePlayerTriggerEnter;
            _footprint.onTriggerExit += HandlePlayerTriggerExit;


            HUD.SetTitle("")
                .SetContent($"Please look for a footprint and step on it.")
                .ShowAll()
                .HideButton();

            Player.TryEnableDesktopInput(3f);
            PlayerEvent.RegisterConfirmHandler(HandleConfirm);
        }

        private void HandleConfirm()
        {
            if (!_readyToConfirm) return;

            isRunning = false;
        }

        private void HandlePlayerTriggerEnter(Collider other)
        {
            var obj = other.transform;
            if (!obj.CompareTag("PlayerCollider")) return;

            _isPlayerOnFootprint = true;

            HUD.FixedRecenter(2f);
            HUD.SetContent("Please align your foot with the footprint.")
                .ShowAll();
        }

        private void HandlePlayerTriggerExit(Collider other)
        {
            var obj = other.transform;
            if (!obj.CompareTag("PlayerCollider")) return;

            _isPlayerOnFootprint = false;
            _readyToConfirm = false;

            HUD.HideAll();
        }

        private void Update()
        {
            if (!isRunning) return;
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
                    HUD.SetContent($"You are not aligned. slowly rotate to your left to align");
                    _readyToConfirm = false;
                    break;
                default:
                    HUD.SetContent($"You are not aligned. slowly rotate to your right to align");
                    _readyToConfirm = false;
                    break;
            }


        }


        protected override void Finish()
        {
            base.Finish();
            ShowEnvironment();
            Player.DisableDesktopInput();
            PlayerEvent.UnregisterConfirmHandler(HandleConfirm);

            if (_footprint)
                Destroy(_footprint.gameObject);
        }

        private float ComputeAngleDifference()
        {
            var playerForward = Player.GetMainCamera().transform.forward;
            var targetForward = _footprint.transform.forward;

            return Vector3.SignedAngle(playerForward, targetForward, Vector3.up);
        }
    }
}
