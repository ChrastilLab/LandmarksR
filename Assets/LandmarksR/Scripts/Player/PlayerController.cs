using System;
using System.Collections;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;
using UnityEngine.XR.Management;

namespace LandmarksR.Scripts.Player
{
    /// <summary>
    /// Enum representing the display modes.
    /// </summary>
    public enum DisplayMode
    {
        Desktop,
        VR
    }

    /// <summary>
    /// Manages the player controller, including switching between VR and desktop modes, logging, and teleportation.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// Reference to the VR player controller.
        /// </summary>
        [SerializeField] public PlayerControllerReference vrPlayerControllerReference;

        /// <summary>
        /// Reference to the desktop player controller.
        /// </summary>
        [SerializeField] public PlayerControllerReference desktopPlayerControllerReference;

        /// <summary>
        /// Reference to the HUD.
        /// </summary>
        [SerializeField] public Hud hud;

        /// <summary>
        /// Reference to the player event controller.
        /// </summary>
        [NotEditable, SerializeField] public PlayerEventController playerEvent;

        /// <summary>
        /// Reference to the first person controller.
        /// </summary>
        [NotEditable, SerializeField] private FirstPersonController firstPersonController;

        private Action _loggingAction;
        private Settings _settings;
        private ExperimentLogger _logger;
        private bool _playerLogging = true;

        /// <summary>
        /// Singleton instance of the PlayerController.
        /// </summary>
        public static PlayerController Instance { get; private set; }

        /// <summary>
        /// Unity Awake method. Ensures there is only one instance of the PlayerController.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Unity Start method. Initializes the player controller and starts logging.
        /// </summary>
        private void Start()
        {
            _settings = Settings.Instance;
            _logger = ExperimentLogger.Instance;
            SwitchDisplayMode(_settings.displayReference.displayMode);

            var waitTime = 0.001f * _settings.logging.loggingIntervalInMillisecond;
            _logger.I("player", $"Logging Interval: {waitTime}");
            // StartCoroutine(PlayerLoggingCoroutine(waitTime));
        }

        #region Logging

        /// <summary>
        /// Starts logging player data.
        /// </summary>
        public void StartPlayerLogging()
        {
            _playerLogging = true;
            StartCoroutine(PlayerLoggingCoroutine());
        }

        /// <summary>
        /// Stops logging player data.
        /// </summary>
        public void StopPlayerLogging()
        {
            _playerLogging = false;
        }

        /// <summary>
        /// Coroutine for logging player data at regular intervals.
        /// </summary>
        /// <param name="interval">The interval between log entries.</param>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator PlayerLoggingCoroutine(float interval = 0.2f)
        {
            while (_playerLogging)
            {
                yield return new WaitForSeconds(interval);
                _loggingAction?.Invoke();
            }
        }

        /// <summary>
        /// Handles logging for VR mode.
        /// </summary>
        private void HandleVRLogging()
        {
            var vrTransform = vrPlayerControllerReference.mainCamera.transform;
            var position = vrTransform.position;
            var rotation = vrTransform.rotation.eulerAngles;
            _logger.I("player", $"Position: {position}|Rotation: {rotation}");
        }

        /// <summary>
        /// Handles logging for desktop mode.
        /// </summary>
        private void HandleDesktopLogging()
        {
            var desktopTransform = desktopPlayerControllerReference.mainCamera.transform;
            var position = desktopTransform.position;
            var rotation = desktopTransform.rotation.eulerAngles;
            _logger.I("player", $"Position: {position}|Rotation: {rotation}");
        }

        #endregion

        /// <summary>
        /// Teleports the player to a specified position and rotation.
        /// </summary>
        /// <param name="position">The position to teleport to.</param>
        /// <param name="rotation">The rotation to teleport to.</param>
        public void Teleport(Vector3 position, Vector3 rotation)
        {
            if (firstPersonController)
            {
                firstPersonController.Teleport(position, rotation);
            }
        }

        /// <summary>
        /// Gets the main camera based on the current display mode.
        /// </summary>
        /// <returns>The main camera.</returns>
        public Camera GetMainCamera()
        {
            return _settings.displayReference.displayMode switch
            {
                DisplayMode.Desktop => desktopPlayerControllerReference.mainCamera,
                DisplayMode.VR => vrPlayerControllerReference.mainCamera,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Switches the display mode between desktop and VR.
        /// </summary>
        /// <param name="displayMode">The display mode to switch to.</param>
        private void SwitchDisplayMode(DisplayMode displayMode)
        {
            switch (displayMode)
            {
                case DisplayMode.Desktop:
                    _settings.displayReference = _settings.desktopDisplay;

                    vrPlayerControllerReference.gameObject.SetActive(false);
                    desktopPlayerControllerReference.gameObject.SetActive(true);

                    firstPersonController = desktopPlayerControllerReference.GetComponent<FirstPersonController>();
                    playerEvent = desktopPlayerControllerReference.GetComponent<PlayerEventController>();

                    _loggingAction = HandleDesktopLogging;
                    break;
                case DisplayMode.VR:
                    _settings.displayReference = _settings.vrDisplay;

                    desktopPlayerControllerReference.gameObject.SetActive(false);
                    vrPlayerControllerReference.gameObject.SetActive(true);

                    firstPersonController = null;
                    playerEvent = vrPlayerControllerReference.GetComponent<PlayerEventController>();

                    StartXR();

                    _loggingAction = HandleVRLogging;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Enables desktop input for the player.
        /// </summary>
        public void TryEnableDesktopInput()
        {
            if (firstPersonController != null)
                firstPersonController.enableControl = true;
        }

        /// <summary>
        /// Enables desktop input for the player after a delay.
        /// </summary>
        /// <param name="delay">The delay before enabling input.</param>
        public void TryEnableDesktopInput(float delay)
        {
            StartCoroutine(TryEnableDesktopInputCoroutine(delay));
        }

        /// <summary>
        /// Coroutine for enabling desktop input after a delay.
        /// </summary>
        /// <param name="delay">The delay before enabling input.</param>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator TryEnableDesktopInputCoroutine(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            TryEnableDesktopInput();
        }

        /// <summary>
        /// Disables desktop input for the player.
        /// </summary>
        public void DisableDesktopInput()
        {
            if (firstPersonController != null)
                firstPersonController.enableControl = false;
        }

        /// <summary>
        /// Starts the XR subsystems.
        /// </summary>
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

        /// <summary>
        /// Stops the XR subsystems.
        /// </summary>
        private static void StopXR()
        {
            if (!XRGeneralSettings.Instance.Manager.isInitializationComplete) return;
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        /// <summary>
        /// Unity OnDisable method. Stops logging when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            _playerLogging = false;
        }

        /// <summary>
        /// Unity OnApplicationQuit method. Stops the XR subsystems when the application quits.
        /// </summary>
        private void OnApplicationQuit()
        {
            StopXR();
        }
    }
}
