using System;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;
using UnityEngine.Assertions;

namespace LandmarksR.Scripts.Experiment
{
    [Serializable]
    public class ExperimentSettings
    {
        public string participantId;
    }


    [Serializable]
    public class DisplaySettings
    {
        public DisplayMode displayMode = DisplayMode.Desktop;
        public HudMode hudMode = HudMode.Follow;
        public float hudDistance = 1.5f;
        public Vector2 hudScreenSize = new(1920f, 1080f);
    }

    [Serializable]
    public class InteractionSettings
    {
        public float hudColliderThickness = 0.05f;
    }

    [Serializable]
    public class CalibrationSettings
    {
        public float controllerHeight = 0.15f; //unit is centimeter
    }

    [Serializable]
    public class SpaceSettings
    {
        /*
         * 4 corners of the space
         *  leftTop ---- rightTop
         *  |                |
         *  leftBottom ---- rightBottom
         */
        [NotEditable] public bool calibrated;
        [NotEditable] public float groundY = 0f;
        [NotEditable] public Vector3 leftTop;
        [NotEditable] public Vector3 rightTop;
        [NotEditable] public Vector3 leftBottom;
        [NotEditable] public Vector3 rightBottom;
        [NotEditable] public Vector3 center;
        [NotEditable] public Vector3 forward;

        public void CalibrateSpace()
        {
            ComputeCenter();
            ComputeForward();
            calibrated = true;
        }

        private void ComputeCenter()
        {
            // average x and z of the corners
            var x = (leftTop.x + rightTop.x + leftBottom.x + rightBottom.x) / 4;
            var z = (leftTop.z + rightTop.z + leftBottom.z + rightBottom.z) / 4;
            center = new Vector3(x, groundY, z);
        }

        private void ComputeForward()
        {
            // use the leftTop and rightTop to compute the forward vector
            var leftTopTemp = new Vector3(leftTop.x, groundY, leftTop.z);
            var rightTopTemp = new Vector3(rightTop.x, groundY, rightTop.z);
            var vec1 = leftTopTemp - center;
            var vec2 = rightTopTemp - center;
            forward = (vec1.normalized + vec2.normalized).normalized;
        }

        public void ApplyToEnvironment()
        {
            // apply the settings to the environment
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

    [Serializable]
    public class LoggingSettings
    {
        public bool localLogging;
        public bool remoteLogging;
        public string remoteStatusUrl;
        public string remoteLogUrl;
        public float loggingIntervalInMillisecond;
    }

    [Serializable]
    public class UISettings
    {
        public float calibrationTriggerTime = 1.25f;
    }


    public class Settings : MonoBehaviour
    {
        public static Settings Instance => _instance ??= BuildConfig();
        private static Settings _instance;

        private static Settings BuildConfig()
        {
            var settings = new GameObject("Settings").AddComponent<Settings>();
            settings.experiment.participantId = "default_participant_id";
            return settings;
        }

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

        private void SwitchDisplayMode(DisplayMode displayMode)
        {
            displayReference = displayMode switch
            {
                DisplayMode.Desktop => desktopDisplay,
                DisplayMode.VR => vrDisplay,
                _ => throw new Exception("Invalid Display Mode")
            };
        }

        public ExperimentSettings experiment = new()
        {
            participantId = "default_participant_0"
        };

        public DisplayMode defaultDisplayMode = DisplayMode.VR;

        public DisplaySettings vrDisplay = new()
        {
            displayMode = DisplayMode.VR,
            hudMode = HudMode.Fixed,
            hudDistance = 3f,
            hudScreenSize = new Vector2(1920f, 1080f)
        };

        public DisplaySettings desktopDisplay = new()
        {
            displayMode = DisplayMode.Desktop,
            hudMode = HudMode.Follow,
            hudDistance = 1.5f,
            hudScreenSize = new Vector2(1920f, 1080f)
        };


        public DisplaySettings displayReference = new();

        public InteractionSettings interaction = new();

        public SpaceSettings space = new();

        public CalibrationSettings calibration = new();

        public LoggingSettings logging = new()
        {
            localLogging = true,
            remoteLogging = false,
            remoteStatusUrl = "http://localhost:3000/status",
            remoteLogUrl = "http://localhost:3000/log",
            loggingIntervalInMillisecond = 200f
        };

        public UISettings ui = new();
    }
}
