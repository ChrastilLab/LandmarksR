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

        protected override void Prepare()
        {
            base.Prepare();
            if (!polePrefab)
            {
                logger.E("calibration", "Pole Prefab is not set.");
            }

            if (!floorIndicatorPrefab)
            {
                logger.E("calibration", "Floor Indicator Prefab is not set.");
            }

            if (!calibrationResultPrefab)
            {
                logger.E("calibration", "Calibration Result Prefab is not set.");
            }

            _rightHandAnchor = playerController.vrPlayerControllerReference.rightHandAnchor;

            // Update HUD
            settings.displayReference.hudMode = HudMode.Follow;
            hud.ApplySettingChanges();
        }

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
                logger.W("Calibration", "Pole positions are not set properly.");
                hud.SetTitle("Calibration Warning")
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
                logger.W("Calibration", "Poles are not set properly.");
                hud.SetTitle("Calibration Warning")
                    .SetContent(
                        "Poles are not set properly. Please make sure you set the poles correctly in the inspector.")
                    .ShowAll();
                return;
            }

            if (polePositions.Count != 4)
            {
                logger.W("Calibration", "Pole positions are not set properly.");
                hud.SetTitle("Calibration Warning")
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
                        settings.space.leftTop = position;
                        break;
                    case PolePosition.RightTop:
                        settings.space.rightTop = position;
                        break;
                    case PolePosition.RightBottom:
                        settings.space.rightBottom = position;
                        break;
                    case PolePosition.LeftBottom:
                        settings.space.leftBottom = position;
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
            settings.space.groundY = _floorIndicator.transform.position.y;
            logger.I("calibration", $"Floor position updated to {settings.space.groundY}.");
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
            position.y -= settings.calibration.controllerHeight;
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

        public void ResetAll()
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

            ResetNode();
        }
    }
}
