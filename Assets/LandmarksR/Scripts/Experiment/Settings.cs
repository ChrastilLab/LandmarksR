using System;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;

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
    public class SpaceSettings
    {
        /*
         * 4 corners of the space
         *  leftTop ---- rightTop
         *  |                |
         *  leftBottom ---- rightBottom
         */
        [NotEditable]
        public float groundY = 0f;
        [NotEditable]
        public Vector3 leftTop;
        [NotEditable]
        public Vector3 rightTop;
        [NotEditable]
        public Vector3 leftBottom;
        [NotEditable]
        public Vector3 rightBottom;
        [NotEditable]
        public Vector3 center;
        [NotEditable]
        public Vector3 forward;
        public Vector3 ComputeCenter()
        {
            // compute a xz plane normal
            // var groundPlaneNormal = new Vector3(0, 1, 0);
            // // compute the diagonals of the space
            // var diagonal1 = leftTop - rightBottom;
            // var diagonal2 = rightTop - leftBottom;
            //
            // // compute the projection of the diagonals on the xz plane
            // var projDiagonal1 = Vector3.ProjectOnPlane(diagonal1, groundPlaneNormal);
            // var projDiagonal2 = Vector3.ProjectOnPlane(diagonal2, groundPlaneNormal);
            //
            // // compute the center of the space by

            // average x and z of the corners
            var x = (leftTop.x + rightTop.x + leftBottom.x + rightBottom.x) / 4;
            var z = (leftTop.z + rightTop.z + leftBottom.z + rightBottom.z) / 4;
            center = new Vector3(x, groundY, z);
            return center;
        }

        public Vector3 ComputeForward()
        {
            // use the leftTop and rightTop to compute the forward vector
            var vec1 = leftTop - center;
            var vec2 = rightTop - center;
            forward =  (vec1.normalized + vec2.normalized).normalized;
            return forward;
        }

        public void ApplyToEnvironment()
        {
            // apply the settings to the environment
            var environment = GameObject.FindGameObjectWithTag("Environment");
            if (!environment) return;

            var environmentTransform = environment.transform;
            environmentTransform.position = center;
            environmentTransform.forward = forward;
        }
    }

    [Serializable]
    public class LoggingSettings
    {
        public bool localLogging;
        public bool remoteLogging;
        public string remoteStatusUrl;
        public string remoteLogUrl;
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

        public ExperimentSettings experiment = new()
        {
            participantId = "default_participant_0"
        };

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

        public DisplayMode defaultDisplayMode = DisplayMode.VR;

        public SpaceSettings space = new();

        public LoggingSettings logging = new()
        {
            localLogging = true,
            remoteLogging = false,
            remoteStatusUrl = "http://localhost:3000/status",
            remoteLogUrl = "http://localhost:3000/log"
        };

        private void SwitchDisplayMode(DisplayMode displayMode)
        {
            displayReference = displayMode switch
            {
                DisplayMode.Desktop => desktopDisplay,
                DisplayMode.VR => vrDisplay,
                _ => throw new Exception("Invalid Display Mode")
            };
        }

        public DisplaySettings displayReference = new();
    }
}
