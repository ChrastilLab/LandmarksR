using System;
using System.Collections;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment;
using TMPro;
using UnityEngine;
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
        private Config _config;
        [NotEditable, SerializeField] private HudMode hudMode;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private Button confirmButton;

        private void Start()
        {
            _config = Config.Instance;
            SwitchHudMode(_config.HudMode);
        }

        public void SetCamera(Camera cam)
        {
            canvas.worldCamera = cam;
        }

        public void SetCameraToFollow()
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }

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

        public Hud SetButtonText(string text)
        {
            confirmButton.GetComponentInChildren<TMP_Text>().text = text;
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

        public Hud ShowButton()
        {
            confirmButton.gameObject.SetActive(true);
            return this;
        }

        public Hud ShowAll()
        {
            panel.SetActive(true);
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

        public Hud HideAll()
        {
            panel.SetActive(false);
            return this;
        }

        public void HideAllAfter(float seconds)
        {
            StartCoroutine(HideAllAfterCoroutine(seconds));
        }

        private IEnumerator HideAllAfterCoroutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            HideAll();
        }


        public Hud SwitchHudMode(HudMode mode)
        {
            switch (mode)
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
            hudMode = HudMode.Follow;
            AdjustScale(_config.HudScreenSize);
            canvas.renderMode = RenderMode.WorldSpace;
        }
        private void SetModeFixed()
        {
            hudMode = HudMode.Fixed;
            AdjustScale(_config.HudScreenSize);
            Recenter(_config.HudDistance);

            canvas.renderMode = RenderMode.WorldSpace;
        }

        private void SetModeOverlay()
        {
            hudMode = HudMode.Overlay;
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
            if (hudMode != HudMode.Follow) return;
            Recenter(_config.HudDistance);
        }

        private void Recenter(float distanceToCam)
        {
            // Get the camera position
            var camTransform = canvas.worldCamera.transform;
            var camPos = camTransform.position;
            var camForward = camTransform.forward;
            var camUp = camTransform.up;

            // Set the position of the canvas to the camera position + the camera forward vector * distanceToCam
            canvas.transform.position = camPos + camForward * distanceToCam;
            canvas.transform.rotation = Quaternion.LookRotation(camForward, camUp);
        }


        public void SetCanvasPosition(Vector3 position, Vector3 lookAt, Vector3 upward)
        {
            if (hudMode != HudMode.Fixed) return;

            canvas.transform.position = position;
            canvas.transform.rotation = Quaternion.LookRotation(lookAt, upward);
        }

        private void AdjustScale(Vector2 size)
        {
            var scaleFactor = CalculateScaleFactor(size);
            canvas.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
        private float CalculateScaleFactor(Vector2 size)
        {
            // This is a simple heuristic. You might need to adjust this formula based on your specific needs
            // and camera settings. This formula assumes a FOV of 60 degrees.
            var h = 2.0f *  Mathf.Tan(0.5f * canvas.worldCamera.fieldOfView * Mathf.Deg2Rad);
            var scaleFactor = h / size.y;
            return scaleFactor;
        }

        private void Update()
        {
            FollowRecenter();
        }
    }
}
