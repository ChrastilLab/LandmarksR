using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace LandmarksR.Scripts.Player
{
    public class Hud : MonoBehaviour
    {
        private HudMode _mode;
        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text defaultText;
        private void Start()
        {
            SetHudMode(HudMode.Overlay);
        }

        public void SetCamera(Camera cam)
        {
            canvas.worldCamera = cam;
        }

        public void SetCameraToFollow()
        {
            canvas.renderMode = RenderMode.WorldSpace;
        }
        
        public void ChangeText(string text)
        {
            defaultText.text = text;
            ShowText();
        }
        
        public void ShowText()
        {
            defaultText.enabled = true;
        }
        
        public void HideText()
        {
            defaultText.enabled = false;
        }
        
        
        private void SetHudMode(HudMode mode)
        {
            switch (mode)
            {
                case HudMode.Overlay:
                    break;
                case HudMode.Follow:
                    break;
                case HudMode.Fixed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _mode = mode;
        }

        private void SetModeFill()
        {
            
        }
        
        private void SetModeFollow()
        {
            
        }
    }

    public enum HudMode
    {
        Overlay,
        Follow,
        Fixed,
    }
}