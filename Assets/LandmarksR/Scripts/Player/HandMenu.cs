using TMPro;
using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    public class HandMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text content;
        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetText(string text)
        {
            content.text = text;
        }
    }
}
