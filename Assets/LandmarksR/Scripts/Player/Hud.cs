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
    /// <summary>
    /// Enum representing the different HUD modes.
    /// </summary>
    public enum HudMode
    {
        Follow,  // Camera Follows Player
        Fixed,   // Fixed Position, requires recenter
        Overlay, // Desktop Only
    }

    /// <summary>
    /// Manages the HUD (Heads-Up Display) for the player, including modes, positioning, and content.
    /// </summary>
    public class Hud : MonoBehaviour
    {
        private Settings _settings;
        private ExperimentLogger _logger;
        private PlayerController _playerController;
        private Camera _camera;

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

        /// <summary>
        /// Unity Start method. Initializes the HUD and sets up initial configurations.
        /// </summary>
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

        /// <summary>
        /// Applies setting changes to the HUD.
        /// </summary>
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

        /// <summary>
        /// Sets the title text of the HUD.
        /// </summary>
        /// <param name="text">The title text.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud SetTitle(string text)
        {
            titleText.text = text;
            return this;
        }

        /// <summary>
        /// Sets the content text of the HUD.
        /// </summary>
        /// <param name="text">The content text.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud SetContent(string text)
        {
            contentText.text = text;
            return this;
        }

        /// <summary>
        /// Clears all text from the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ClearAllText()
        {
            SetTitle("");
            SetContent("");
            return this;
        }

        /// <summary>
        /// Shows the title text on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowTitle()
        {
            titleText.enabled = true;
            return this;
        }

        /// <summary>
        /// Shows the content text on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowContent()
        {
            contentText.enabled = true;
            return this;
        }

        /// <summary>
        /// Shows the instruction panel on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowPanel()
        {
            instructionPanel.gameObject.SetActive(true);
            return this;
        }

        /// <summary>
        /// Hides the instruction panel on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud HidePanel()
        {
            instructionPanel.gameObject.SetActive(false);
            return this;
        }

        /// <summary>
        /// Hides the title text on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud HideTitle()
        {
            titleText.enabled = false;
            return this;
        }

        /// <summary>
        /// Hides the content text on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud HideContent()
        {
            contentText.enabled = false;
            return this;
        }

        /// <summary>
        /// Hides the confirmation button on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud HideButton()
        {
            confirmButton.gameObject.SetActive(false);
            return this;
        }

        /// <summary>
        /// Sets the opacity of the instruction panel.
        /// </summary>
        /// <param name="opacity">The desired opacity.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud SetOpacity(float opacity)
        {
            var color = instructionPanel.color;
            color.a = opacity;
            instructionPanel.color = color;
            return this;
        }

        /// <summary>
        /// Sets the alignment of the content text.
        /// </summary>
        /// <param name="alignment">The desired text alignment.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud SetContentAlignment(TextAlignmentOptions alignment)
        {
            contentText.alignment = alignment;
            return this;
        }

        #endregion

        #region Confirmation Button

        /// <summary>
        /// Sets the text of the confirmation button.
        /// </summary>
        /// <param name="text">The button text.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud SetButtonText(string text)
        {
            confirmButton.GetComponentInChildren<TMP_Text>().text = text;
            return this;
        }

        /// <summary>
        /// Shows the confirmation button on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowButton()
        {
            confirmButton.gameObject.SetActive(true);
            return this;
        }

        #endregion

        #region Progress Bar

        /// <summary>
        /// Shows the progress bar on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowProgressBar()
        {
            progressBar.gameObject.SetActive(true);
            progressBar.SetMaxWidth(_settings.displayReference?.hudScreenSize.x ?? 1080);
            return this;
        }

        /// <summary>
        /// Hides the progress bar on the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud HideProgressBar()
        {
            progressBar.gameObject.SetActive(false);
            return this;
        }

        /// <summary>
        /// Sets the progress value of the progress bar.
        /// </summary>
        /// <param name="value">The progress value.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud SetProgress(float value)
        {
            progressBar.SetProgress(value);
            return this;
        }

        #endregion

        /// <summary>
        /// Shows all elements of the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowAll()
        {
            ShowPanel();
            ShowTitle();
            ShowContent();
            ShowAllLayer();
            return this;
        }

        /// <summary>
        /// Hides all elements of the HUD.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud HideAll()
        {
            HidePanel();
            return this;
        }

        #endregion

        /// <summary>
        /// Hides all elements of the HUD after a specified time.
        /// </summary>
        /// <param name="seconds">The delay in seconds before hiding the HUD.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud HideAllAfter(float seconds)
        {
            StartCoroutine(HideAllAfterCoroutine(seconds));
            return this;
        }

        /// <summary>
        /// Coroutine to hide all elements of the HUD after a delay.
        /// </summary>
        /// <param name="seconds">The delay in seconds.</param>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator HideAllAfterCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            HideAll();
        }

        /// <summary>
        /// Shows all layers on the HUD's canvas.
        /// </summary>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowAllLayer()
        {
            canvas.worldCamera.cullingMask = -1;
            return this;
        }

        /// <summary>
        /// Shows specified layers on the HUD's canvas.
        /// </summary>
        /// <param name="layers">The layers to show.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowLayers(IEnumerable<string> layers)
        {
            foreach (var layer in layers)
            {
                ShowLayer(layer);
            }

            return this;
        }

        /// <summary>
        /// Hides specified layer on the HUD's canvas.
        /// </summary>
        /// <param name="layers">The layers to hide.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud HideLayers(IEnumerable<string> layers)
        {
            foreach (var layer in layers)
            {
                HideLayer(layer);
            }

            return this;
        }

        /// <summary>
        /// Hides a specific layer on the HUD's canvas.
        /// </summary>
        /// <param name="layerName">The name of the layer to hide.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud HideLayer(string layerName)
        {
            canvas.worldCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layerName));
            return this;
        }

        /// <summary>
        /// Shows a specific layer on the HUD's canvas.
        /// </summary>
        /// <param name="layerName">The name of the layer to show.</param>
        /// <returns>The current Hud instance.</returns>
        public Hud ShowLayer(string layerName)
        {
            canvas.worldCamera.cullingMask |= (1 << LayerMask.NameToLayer(layerName));
            return this;
        }

        /// <summary>
        /// Switches the HUD mode.
        /// </summary>
        /// <param name="mode">The desired HUD mode.</param>
        /// <returns>The current Hud instance.</returns>
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

        /// <summary>
        /// Recenters the HUD to a specified distance when in fixed mode.
        /// </summary>
        /// <param name="distanceToCam">The distance to the camera.</param>
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

        /// <summary>
        /// Adjusts the scale of the HUD based on the screen size.
        /// </summary>
        /// <param name="size"></param>
        private void AdjustScale(Vector2? size)
        {
            if (!size.HasValue) return;

            var h = CalculateCanvasHeight();
            var canvasScale = h / size.Value.y;
            SetTransformScale(canvas.transform, new Vector3(canvasScale, canvasScale, canvasScale));

            var w = canvasScale * size.Value.x;
            SetTransformScale(colliderTransform, new Vector3(w, h, _settings.interaction.hudColliderThickness));
        }

        /// <summary>
        /// Calculates the height of the canvas based on the camera field of view.
        /// </summary>
        /// <returns></returns>
        private float CalculateCanvasHeight()
        {
            return 2.0f * Mathf.Tan(0.5f * _camera.fieldOfView * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Unity Update method. Ensures the HUD follows the player if in follow mode.
        /// </summary>
        private void Update()
        {
            FollowRecenter();
        }
    }
}
