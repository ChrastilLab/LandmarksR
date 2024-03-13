using System;
using System.Collections;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;
using UnityEngine.XR.Management;

namespace LandmarksR.Scripts.Player
{
    public enum DisplayMode
    {
        Desktop,
        VR
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] public PlayerControllerReference vrPlayerControllerReference;
        [SerializeField] public PlayerControllerReference desktopPlayerControllerReference;
        [SerializeField] public Hud hud;
        [NotEditable, SerializeField] public PlayerEventController playerEvent;
        [NotEditable, SerializeField] private FirstPersonController firstPersonController;

        private Action _loggingAction;


        private Settings _settings;
        private ExperimentLogger _logger;
        private bool _playerLogging = true;

        private void Start()
        {
            _settings = Settings.Instance;
            _logger = ExperimentLogger.Instance;
            SwitchDisplayMode(_settings.displayReference.displayMode);

            var waitTime = 0.001f * _settings.logging.loggingIntervalInMillisecond;
            _logger.I("player", $"Logging Interval: {waitTime}");
            StartCoroutine(PlayerLoggingCoroutine(waitTime));
        }

        private IEnumerator PlayerLoggingCoroutine(float interval = 0.2f)
        {
            while (_playerLogging)
            {
                yield return new WaitForSeconds(interval);
                _loggingAction?.Invoke();
            }
        }

        private void HandleVRLogging()
        {
            var vrTransform = hud.GetCamera().transform;
            var position = vrTransform.position;
            var rotation = vrTransform.rotation.eulerAngles;
            _logger.I("player", $"Position: {position}|Rotation: {rotation}");
        }

        private void HandleDesktopLogging()
        {
            var desktopTransform = hud.GetCamera().transform;
            var position = desktopTransform.position;
            var rotation = desktopTransform.rotation.eulerAngles;
            _logger.I("player", $"Position: {position}|Rotation: {rotation}");
        }

        public void Teleport(Vector3 position, Vector3 rotation)
        {
            if (firstPersonController)
            {
                firstPersonController.Teleport(position, rotation);
            }
        }

        public void SwitchDisplayMode(DisplayMode displayMode)
        {
            switch (displayMode)
            {
                case DisplayMode.Desktop:
                    _settings.displayReference = _settings.desktopDisplay;

                    vrPlayerControllerReference.gameObject.SetActive(false);
                    desktopPlayerControllerReference.gameObject.SetActive(true);
                    hud.SetCamera(desktopPlayerControllerReference.mainCamera);
                    hud.UpdateSettings(_settings);

                    firstPersonController = desktopPlayerControllerReference.GetComponent<FirstPersonController>();
                    playerEvent = desktopPlayerControllerReference.GetComponent<PlayerEventController>();

                    _loggingAction = HandleDesktopLogging;
                    break;
                case DisplayMode.VR:
                    _settings.displayReference = _settings.vrDisplay;

                    desktopPlayerControllerReference.gameObject.SetActive(false);
                    vrPlayerControllerReference.gameObject.SetActive(true);
                    hud.SetCamera(vrPlayerControllerReference.mainCamera);
                    hud.UpdateSettings(_settings);

                    firstPersonController = null;
                    playerEvent = vrPlayerControllerReference.GetComponent<PlayerEventController>();

                    StartXR();

                    _loggingAction = HandleVRLogging;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



        public void TryEnableDesktopInput()
        {
            if (firstPersonController != null)
                firstPersonController.enableControl = true;
        }

        public void TryEnableDesktopInput(float delay)
        {
            StartCoroutine(TryEnableDesktopInputCoroutine(delay));
        }

        private IEnumerator TryEnableDesktopInputCoroutine(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            TryEnableDesktopInput();
        }

        public void DisableDesktopInput()
        {
            if (firstPersonController != null)
                firstPersonController.enableControl = false;
        }

        private static void StartXR()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }

            XRGeneralSettings.Instance.Manager.InitializeLoaderSync();
            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("No XR Loader is active. Please ensure that your XR project is set up correctly.");
            }
            else
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        private static void StopXR()
        {
            if (!XRGeneralSettings.Instance.Manager.isInitializationComplete) return;
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        private void OnDisable()
        {
            _playerLogging = false;
        }

        private void OnApplicationQuit()
        {
            StopXR();
        }
    }
}
