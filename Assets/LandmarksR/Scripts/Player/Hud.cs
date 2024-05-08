using System;
using System.Collections;
using System.Collections.Generic;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment;
using LandmarksR.Scripts.Experiment.Log;
using LandmarksR.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LandmarksR.Scripts.Player
{
    public enum HudMode
    {
        Follow, // Camera Follows Player
        Fixed, // Fixed Position, requires recenter
        Overlay, // Desktop Only
    }

    public class Hud : MonoBehaviour
    {
        private Settings _settings;
        private ExperimentLogger _logger;
        private PlayerController _playerController;
        [NotEditable, SerializeField] private HudMode hudMode;
        [SerializeField] private Transform hudTransform;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image instructionPanel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private ProgressBar progressBar;

        [Header("Interactive Properties")]
        [SerializeField] private Transform colliderTransform;
        [SerializeField] private Transform planeSurfaceTransform;

        private Camera _camera;

        private void Start()
        {
            Assert.IsNotNull(hudTransform, "HUD Transform is not set");
            Assert.IsNotNull(canvas, "Canvas is not set");
            Assert.IsNotNull(instructionPanel, "Panel is not set");
            Assert.IsNotNull(colliderTransform, "Box Collider Transform is not set");
            Assert.IsNotNull(planeSurfaceTransform, "Plane Surface Transform is not set");

            _settings = Settings.Instance;
            _logger = ExperimentLogger.Instance;
            _playerController = PlayerController.Instance;

            Assert.IsNotNull(_settings, "Failed to obtain Settings instance");
            Assert.IsNotNull(_logger, "Failed to obtain ExperimentLogger instance");
            Assert.IsNotNull(_playerController, "Failed to obtain PlayerController instance");

            _camera = _playerController.GetMainCamera();
            SwitchHudMode(_settings.displayReference?.hudMode);

            StartCoroutine(WaitForRecenter());
        }

        public void ApplySettingChanges()
        {
            if (!_settings)
            {
                _logger.E("hud", "Applying Setting Changes Failed");
                return;
            }
            SwitchHudMode(_settings.displayReference?.hudMode);
        }

        #region Canvas Components

        #region Text
        public Hud SetTitle(string text)
        {
            titleText.text = text;
            return this;
        }


        public Hud SetContent(string text)
        {
            contentText.text = text;
            return this;
        }

        public Hud ClearAllText()
        {
            SetTitle("");
            SetContent("");
            return this;
        }

        public Hud ShowTitle()
        {
            titleText.enabled = true;
            return this;
        }

        public Hud ShowContent()
        {
            contentText.enabled = true;
            return this;
        }

        public Hud ShowPanel()
        {
            instructionPanel.gameObject.SetActive(true);
            return this;
        }

        public Hud HidePanel()
        {
            instructionPanel.gameObject.SetActive(false);
            return this;
        }

        public Hud HideTitle()
        {
            titleText.enabled = false;
            return this;
        }

        public Hud HideContent()
        {
            contentText.enabled = false;
            return this;
        }

        public Hud HideButton()
        {
            confirmButton.gameObject.SetActive(false);
            return this;
        }


        public Hud SetOpacity(float opacity)
        {
            var color = instructionPanel.color;
            color.a = opacity;
            instructionPanel.color = color;
            return this;
        }

        public Hud SetContentAlignment(TextAlignmentOptions alignment)
        {
            contentText.alignment = alignment;
            return this;
        }

        #endregion

        #region Confirmation Button
        public Hud SetButtonText(string text)
        {
            confirmButton.GetComponentInChildren<TMP_Text>().text = text;
            return this;
        }
        public Hud ShowButton()
        {
            confirmButton.gameObject.SetActive(true);
            return this;
        }
        #endregion

        # region Progress Bar
        public Hud ShowProgressBar()
        {
            progressBar.gameObject.SetActive(true);
            progressBar.SetMaxWidth(_settings.displayReference?.hudScreenSize.x ?? 1080);
            return this;
        }

        public Hud HideProgressBar()
        {
            progressBar.gameObject.SetActive(false);
            return this;
        }

        public Hud SetProgress(float value)
        {
            progressBar.SetProgress(value);
            return this;
        }

        #endregion


        public Hud ShowAll()
        {
            ShowPanel();
            ShowTitle();
            ShowContent();
            ShowAllLayer();
            return this;
        }

        public Hud HideAll()
        {
            HidePanel();
            return this;
        }

        #endregion

        public Hud HideAllAfter(float seconds)
        {
            StartCoroutine(HideAllAfterCoroutine(seconds));
            return this;
        }

        private IEnumerator HideAllAfterCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            HideAll();
        }

        public Hud ShowAllLayer()
        {
            canvas.worldCamera.cullingMask = -1;
            return this;
        }

        public Hud ShowLayers(IEnumerable<string> layers)
        {
            foreach (var layer in layers)
            {
                ShowLayer(layer);
            }

            return this;
        }

        public Hud HideLayers(IEnumerable<string> layers)
        {
            foreach (var layer in layers)
            {
                HideLayer(layer);
            }

            return this;
        }


        public Hud HideLayer(string layerName)
        {
            canvas.worldCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
            return this;
        }

        public Hud ShowLayer(string layerName)
        {
            canvas.worldCamera.cullingMask |= (1 << LayerMask.NameToLayer(layerName));
            return this;
        }



        public Hud SwitchHudMode(HudMode? mode)
        {
            if (!mode.HasValue) return this;
            switch (mode.Value)
            {
                case HudMode.Follow:
                    SetModeFollow();
                    break;
                case HudMode.Fixed:
                    SetModeFixed();
                    break;
                case HudMode.Overlay:
                    SetModeOverlay();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return this;
        }


        private void SetModeFollow()
        {
            if (!_settings) return;
            hudMode = HudMode.Follow;
            _logger.I("hud", "SetModeFollow");

            AdjustScale(_settings.displayReference?.hudScreenSize);
            canvas.renderMode = RenderMode.WorldSpace;
        }
        private void SetModeFixed()
        {
            if (!_settings) return;
            hudMode = HudMode.Fixed;
            _logger.I("hud", "SetModeFixed");

            AdjustScale(_settings.displayReference?.hudScreenSize);
            Recenter(_settings.displayReference?.hudDistance);

            canvas.renderMode = RenderMode.WorldSpace;
        }

        private void SetModeOverlay()
        {
            hudMode = HudMode.Overlay;
            _logger.I("hud", "SetModeOverlay");

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Fixed Mode related methods
        public void FixedRecenter(float distanceToCam)
        {
            if (hudMode != HudMode.Fixed) return;
            Recenter(distanceToCam);
        }

        private void FollowRecenter()
        {
            if (!_settings) return;
            if (hudMode != HudMode.Follow) return;
            Recenter(_settings.displayReference?.hudDistance);
        }

        private void Recenter(float? distanceToCam)
        {
            if (!distanceToCam.HasValue) return;

            // Get the camera position
            var camTransform = _camera.transform;
            var camPosition = camTransform.position;
            var camForward = camTransform.forward;
            var camUp = camTransform.up;

            SetTransformPosition(canvas.transform, camPosition + camForward * distanceToCam.Value, camForward, camUp);
            SetTransformPosition(colliderTransform, camPosition + camForward * distanceToCam.Value, camForward, camUp);
            SetTransformPosition(planeSurfaceTransform, camPosition + camForward * distanceToCam.Value, camForward, camUp);
        }

        private IEnumerator WaitForRecenter()
        {
            yield return new WaitUntil(() => _camera.transform.localPosition != Vector3.zero);
            Recenter(_settings.displayReference?.hudDistance);
        }


        private static void SetTransformPosition(Transform tr, Vector3 position, Vector3 lookAt, Vector3 upward)
        {
            tr.position = position;
            tr.rotation = Quaternion.LookRotation(lookAt, upward);
        }

        private static void SetTransformScale(Transform tr, Vector3 scale)
        {
            tr.localScale = scale;
        }

        private void AdjustScale(Vector2? size)
        {
            if (!size.HasValue) return;

            var h = CalculateCanvasHeight();
            var canvasScale = h / size.Value.y;
            SetTransformScale(canvas.transform, new Vector3(canvasScale, canvasScale, canvasScale));

            var w = canvasScale * size.Value.x;
            SetTransformScale(colliderTransform, new Vector3(w, h, _settings.interaction.hudColliderThickness));
        }
        private float CalculateCanvasHeight()
        {
            return 2.0f *  Mathf.Tan(0.5f * _camera.fieldOfView * Mathf.Deg2Rad);
        }


        private void Update()
        {
            FollowRecenter();
        }
    }
}
