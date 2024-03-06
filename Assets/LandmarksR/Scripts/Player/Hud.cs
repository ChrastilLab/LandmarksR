using System;
using System.Collections;
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
        private HudMode _mode;
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

        public void ChangeTitle(string text)
        {
            titleText.text = text;
        }

        public void ChangeContent(string text)
        {
            contentText.text = text;
        }

        public void ChangeButtonText(string text)
        {
            confirmButton.GetComponentInChildren<TMP_Text>().text = text;
        }


        public void ShowTitle()
        {
            titleText.enabled = true;
        }

        public void ShowContent()
        {
            contentText.enabled = true;
        }

        public void ShowButton()
        {
            confirmButton.gameObject.SetActive(true);
        }

        public void ShowAll()
        {
            panel.SetActive(true);
        }

        public void HideTitle()
        {
            titleText.enabled = false;
        }

        public void HideContent()
        {
            contentText.enabled = false;
        }

        public void HideButton()
        {
            confirmButton.gameObject.SetActive(false);
        }

        public void HideAll()
        {
            panel.SetActive(false);
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


        public void SwitchHudMode(HudMode mode)
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

            _mode = mode;
        }


        private void SetModeFollow()
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
        private void SetModeFixed()
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }

        private void SetModeOverlay()
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }
}
