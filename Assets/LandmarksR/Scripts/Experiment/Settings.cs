using System;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace LandmarksR.Scripts.Experiment
{
    /// <summary>
    /// Represents the settings for the experiment.
    /// </summary>
    [Serializable]
    public class ExperimentSettings
    {
        /// <summary>
        /// The participant ID for the experiment.
        /// </summary>
        public string participantId;
    }

    /// <summary>
    /// Represents the display settings for the experiment.
    /// </summary>
    [Serializable]
    public class DisplaySettings
    {
        /// <summary>
        /// The display mode (e.g., Desktop, VR).
        /// </summary>
        public DisplayMode displayMode = DisplayMode.Desktop;

        /// <summary>
        /// The HUD mode (e.g., Follow, Fixed).
        /// </summary>
        public HudMode hudMode = HudMode.Follow;

        /// <summary>
        /// The distance of the HUD from the player.
        /// </summary>
        public float hudDistance = 1.5f;

        /// <summary>
        /// The screen size of the HUD.
        /// </summary>
        public Vector2 hudScreenSize = new Vector2(1920f, 1080f);
    }

    /// <summary>
    /// Represents the interaction settings for the experiment.
    /// </summary>
    [Serializable]
    public class InteractionSettings
    {
        /// <summary>
        /// The thickness of the HUD collider.
        /// </summary>
        public float hudColliderThickness = 0.05f;
    }

    /// <summary>
    /// Represents the calibration settings for the experiment.
    /// </summary>
    [Serializable]
    public class CalibrationSettings
    {
        /// <summary>
        /// The height of the controller in centimeters.
        /// </summary>
        public float controllerHeight = 0.15f;
    }

    /// <summary>
    /// Represents the space settings for the experiment.
    /// </summary>
    [Serializable]
    public class SpaceSettings
    {
        /// <summary>
        /// Indicates whether the space is calibrated.
        /// </summary>
        [NotEditable] public bool calibrated;

        /// <summary>
        /// The Y coordinate of the ground.
        /// </summary>
        [NotEditable] public float groundY;

        /// <summary>
        /// The position of the left top corner of the space.
        /// </summary>
        [NotEditable] public Vector3 leftTop;

        /// <summary>
        /// The position of the right top corner of the space.
        /// </summary>
        [NotEditable] public Vector3 rightTop;

        /// <summary>
        /// The position of the left bottom corner of the space.
        /// </summary>
        [NotEditable] public Vector3 leftBottom;

        /// <summary>
        /// The position of the right bottom corner of the space.
        /// </summary>
        [NotEditable] public Vector3 rightBottom;

        /// <summary>
        /// The center position of the space.
        /// </summary>
        [NotEditable] public Vector3 center;

        /// <summary>
        /// The forward direction of the space.
        /// </summary>
        [NotEditable] public Vector3 forward;

        /// <summary>
        /// Calibrates the space by computing the center and forward direction.
        /// </summary>
        public void CalibrateSpace()
        {
            ComputeCenter();
            ComputeForward();
            calibrated = true;
        }

        /// <summary>
        /// Computes the center of the space.
        /// </summary>
        private void ComputeCenter()
        {
            // Average x and z of the corners
            var x = (leftTop.x + rightTop.x + leftBottom.x + rightBottom.x) / 4;
            var z = (leftTop.z + rightTop.z + leftBottom.z + rightBottom.z) / 4;
            center = new Vector3(x, groundY, z);
        }

        /// <summary>
        /// Computes the forward direction of the space.
        /// </summary>
        private void ComputeForward()
        {
            // Use the leftTop and rightTop to compute the forward vector
            var leftTopTemp = new Vector3(leftTop.x, groundY, leftTop.z);
            var rightTopTemp = new Vector3(rightTop.x, groundY, rightTop.z);
            var vec1 = leftTopTemp - center;
            var vec2 = rightTopTemp - center;
            forward = (vec1.normalized + vec2.normalized).normalized;
        }

        /// <summary>
        /// Applies the space settings to the environment.
        /// </summary>
        public void ApplyToEnvironment()
        {
            // Apply the settings to the environment
            var environment = GameObject.FindGameObjectWithTag("Environment");
            Assert.IsNotNull(environment, "Can't find environment object, please add a GameObject with the tag 'Environment'");

            var calibrationSpace = GameObject.FindGameObjectWithTag("Calibration");
            Assert.IsNotNull(calibrationSpace, "Can't find calibration object, please add a GameObject with the tag 'Calibration'");

            var environmentTransform = environment.transform;
            environmentTransform.position = center;
            environmentTransform.forward = forward;

            var calibrationSpaceTransform = calibrationSpace.transform;
            calibrationSpaceTransform.position = center;
            calibrationSpaceTransform.forward = forward;
        }
    }

    /// <summary>
    /// Represents the logging settings for the experiment.
    /// </summary>
    [Serializable]
    public class LoggingSettings
    {
        /// <summary>
        /// Indicates whether local logging is enabled.
        /// </summary>
        public bool localLogging;

        /// <summary>
        /// Indicates whether remote logging is enabled.
        /// </summary>
        public bool remoteLogging;

        /// <summary>
        /// The URL for the remote logging status.
        /// </summary>
        public string remoteStatusUrl;

        /// <summary>
        /// The URL for remote logging.
        /// </summary>
        public string remoteLogUrl;

        /// <summary>
        /// The interval for logging data in milliseconds.
        /// </summary>
        public float loggingIntervalInMillisecond;

        /// <summary>
        /// The delimiter for the data file.
        /// </summary>
        public string dataFileDelimiter;

        /// <summary>
        /// The extension for the data file.
        /// </summary>
        public string dataFileExtension;
    }

    /// <summary>
    /// Represents the UI settings for the experiment.
    /// </summary>
    [Serializable]
    public class UISettings
    {
        /// <summary>
        /// The time in seconds for triggering calibration.
        /// </summary>
        public float calibrationTriggerTime = 1.25f;
    }

    /// <summary>
    /// Manages the settings for the experiment, ensuring there is only one instance.
    /// </summary>
    public class Settings : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance of the Settings class.
        /// </summary>
        public static Settings Instance => _instance ??= BuildConfig();
        private static Settings _instance;

        /// <summary>
        /// Builds the singleton instance of the Settings class.
        /// </summary>
        /// <returns>A new instance of the Settings class.</returns>
        private static Settings BuildConfig()
        {
            var settings = new GameObject("Settings").AddComponent<Settings>();
            settings.experiment.participantId = "default_participant_id";
            return settings;
        }

        /// <summary>
        /// Unity Awake method. Ensures there is only one instance of the Settings class.
        /// </summary>
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
                SwitchDisplayMode(defaultDisplayMode);
                DontDestroyOnLoad(this);
            }
        }

        /// <summary>
        /// Switches the display mode based on the provided mode.
        /// </summary>
        /// <param name="displayMode">The display mode to switch to.</param>
        private void SwitchDisplayMode(DisplayMode displayMode)
        {
            displayReference = displayMode switch
            {
                DisplayMode.Desktop => desktopDisplay,
                DisplayMode.VR => vrDisplay,
                _ => throw new Exception("Invalid Display Mode")
            };
        }

        /// <summary>
        /// The experiment settings.
        /// </summary>
        public ExperimentSettings experiment = new()
        {
            participantId = "default_participant_0"
        };

        /// <summary>
        /// The default display mode.
        /// </summary>
        public DisplayMode defaultDisplayMode = DisplayMode.VR;

        /// <summary>
        /// The VR display settings.
        /// </summary>
        public DisplaySettings vrDisplay = new()
        {
            displayMode = DisplayMode.VR,
            hudMode = HudMode.Fixed,
            hudDistance = 3f,
            hudScreenSize = new Vector2(1920f, 1080f)
        };

        /// <summary>
        /// The desktop display settings.
        /// </summary>
        public DisplaySettings desktopDisplay = new()
        {
            displayMode = DisplayMode.Desktop,
            hudMode = HudMode.Follow,
            hudDistance = 1.5f,
            hudScreenSize = new Vector2(1920f, 1080f)
        };

        /// <summary>
        /// The reference to the current display settings.
        /// </summary>
        public DisplaySettings displayReference = new();

        /// <summary>
        /// The interaction settings.
        /// </summary>
        public InteractionSettings interaction = new();

        /// <summary>
        /// The space settings.
        /// </summary>
        public SpaceSettings space = new();

        /// <summary>
        /// The calibration settings.
        /// </summary>


 public CalibrationSettings calibration = new();

        /// <summary>
        /// The logging settings.
        /// </summary>
        public LoggingSettings logging = new()
        {
            localLogging = true,
            remoteLogging = false,
            remoteStatusUrl = "http://localhost:3000/status",
            remoteLogUrl = "http://localhost:3000/log",
            loggingIntervalInMillisecond = 200f,
            dataFileDelimiter = ",",
            dataFileExtension = "csv"
        };

        /// <summary>
        /// The UI settings.
        /// </summary>
        public UISettings ui = new();
    }
}
