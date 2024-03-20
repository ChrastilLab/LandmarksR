using LandmarksR.Scripts.Attributes;
using UnityEngine;

namespace LandmarksR.Scripts.UI
{
    public class ProgressBar : MonoBehaviour
    {

        [SerializeField] private RectTransform progressBar;
        [NotEditable, SerializeField] private float progress;
        [NotEditable, SerializeField] private float maxWidth;

        private void Start()
        {
            if (!progressBar)
            {
                Debug.LogError("Progress Bar child GameObject is missing.");
            }
        }

        public void SetMaxWidth(float value)
        {
            maxWidth = value;
        }

        public void SetProgress(float value)
        {
            progress = value;
            var size = progressBar.sizeDelta;
            size.x = maxWidth * progress;
            progressBar.sizeDelta = new Vector2(size.x * progress, size.y);
        }
    }
}
