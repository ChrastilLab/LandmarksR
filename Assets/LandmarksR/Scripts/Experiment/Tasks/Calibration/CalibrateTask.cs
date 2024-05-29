using System;
using System.Collections.Generic;
using System.Linq;
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

    public class CalibrateTask : CollectionTask
    {
        // Pole Variables
        [Header("Calibration Settings")] [SerializeField]
        private GameObject polePrefab;

        [SerializeField] private List<PolePosition> polePositions = new()
        {
            PolePosition.LeftTop,
            PolePosition.RightTop,
            PolePosition.RightBottom,
            PolePosition.LeftBottom
        };

        private readonly List<GameObject> poles = new();

        [SerializeField] private GameObject floorIndicatorPrefab;

        private GameObject _floorIndicator;

        // Calibration Result Indicator
        [SerializeField] private GameObject calibrationResultPrefab;
        private GameObject _calibrationResultIndicator;

        // Other references
        private GameObject _rightHandAnchor;


        public void AddPole()
        {
            var position = _rightHandAnchor.transform.position;
            position.y = 0;
            var pole = Instantiate(polePrefab, position, Quaternion.identity);
            poles.Add(pole);
        }


        public void RemoveLastPole()
        {
            if (poles.Count <= 0) return;
            Destroy(poles[^1]);
            poles.RemoveAt(poles.Count - 1);
        }

        public string GetCurrentPolePosition()
        {
            if (polePositions.Count != 4)
            {
                Logger.W("Calibration", "Pole positions are not set properly.");
                HUD.SetTitle("Calibration Warning")
                    .SetContent(
                        "Pole positions are not set properly. Please make sure you set the pole positions correctly in the inspector.")
                    .ShowAll();
                return "";
            }

            var position = polePositions[poles.Count];
            return PolePositionToString(position);
        }

        private static string PolePositionToString(PolePosition position) => position switch
        {
            PolePosition.LeftTop => "Left Top",
            PolePosition.RightTop => "Right Top",
            PolePosition.RightBottom => "Right Bottom",
            PolePosition.LeftBottom => "Left Bottom",
            _ => throw new ArgumentOutOfRangeException()
        };

        public void UpdatePolePositionsInSettings()
        {
            if (poles.Count != 4)
            {
                Logger.W("Calibration", "Poles are not set properly.");
                HUD.SetTitle("Calibration Warning")
                    .SetContent(
                        "Poles are not set properly. Please make sure you set the poles correctly in the inspector.")
                    .ShowAll();
                return;
            }

            if (polePositions.Count != 4)
            {
                Logger.W("Calibration", "Pole positions are not set properly.");
                HUD.SetTitle("Calibration Warning")
                    .SetContent(
                        "Pole positions are not set properly. Please make sure you set the pole positions correctly in the inspector.")
                    .ShowAll();
                return;
            }

            foreach (var (positionType, pole) in polePositions.Zip(poles, (position, pole) => (position, pole)))
            {
                var position = pole.transform.position;
                switch (positionType)
                {
                    case PolePosition.LeftTop:
                        Settings.space.leftTop = position;
                        break;
                    case PolePosition.RightTop:
                        Settings.space.rightTop = position;
                        break;
                    case PolePosition.RightBottom:
                        Settings.space.rightBottom = position;
                        break;
                    case PolePosition.LeftBottom:
                        Settings.space.leftBottom = position;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // Floor Indicator
        public void UpdateFloorPositionInSettings()
        {
            if (!_floorIndicator) return;
            Settings.space.groundY = _floorIndicator.transform.position.y;
            Logger.I("calibration", $"Floor position updated to {Settings.space.groundY}.");
        }

        public void InitializeFloorIndicator()
        {
            if (_floorIndicator)
            {
                Destroy(_floorIndicator);
            }

            _floorIndicator = Instantiate(floorIndicatorPrefab, Vector3.zero, Quaternion.identity);
        }

        public void UpdateFloorIndicator()
        {
            if (!_floorIndicator) return;
            var position = _rightHandAnchor.transform.position;
            position.y -= Settings.calibration.controllerHeight;
            _floorIndicator.transform.position = position;
        }

        public void RemoveFloorIndicator()
        {
            if (_floorIndicator)
            {
                Destroy(_floorIndicator);
            }
        }


        // Calibration

        public void ShowCalibrationResultIndicator(Vector3 position, Vector3 forward)
        {
            if (_calibrationResultIndicator)
            {
                Destroy(_calibrationResultIndicator);
            }

            _calibrationResultIndicator =
                Instantiate(calibrationResultPrefab, position, Quaternion.LookRotation(forward));
        }

        public void RemoveCalibrationResultIndicator()
        {
            if (_calibrationResultIndicator)
            {
                Destroy(_calibrationResultIndicator);
            }
        }

        public void ComputeCalibration()
        {
            UpdateFloorPositionInSettings();
            UpdatePolePositionsInSettings(); // Update the pole positions in the settings
            Settings.space.CalibrateSpace(); // Calibrate the space based on the pole positions
            ShowCalibrationResultIndicator(Settings.space.center, Settings.space.forward);
        }

        private void DeleteAllIndicators()
        {
            foreach (var pole in poles)
            {
                Destroy(pole);
            }

            poles.Clear();
            if (_calibrationResultIndicator)
            {
                Destroy(_calibrationResultIndicator);
            }

            if (_floorIndicator)
            {
                Destroy(_floorIndicator);
            }
        }
        public void ResetAll()
        {
            DeleteAllIndicators();
            ResetNode();
        }

        protected override void Prepare()
        {
            SetTaskType(TaskType.Structural);
            base.Prepare();
            _rightHandAnchor = Player.vrPlayerControllerReference.rightHandAnchor;

            // Update HUD
            Settings.displayReference.hudMode = HudMode.Follow;
            HUD.ApplySettingChanges();
            HUD.HideButton()
               .ShowProgressBar();
        }

        public override void Finish()
        {
            base.Finish();
            var environment = GameObject.FindGameObjectWithTag("Environment");
            if (environment)
            {
                foreach (Transform tr in environment.transform)
                {
                    tr.gameObject.SetActive(true);
                }
            }

            var calibrationSpace = GameObject.FindGameObjectWithTag("Calibration");
            calibrationSpace.SetActive(false);

            HUD.HideProgressBar();
            DeleteAllIndicators();
        }
    }
}
