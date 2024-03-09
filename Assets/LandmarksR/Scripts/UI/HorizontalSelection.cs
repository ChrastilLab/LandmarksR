using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LandmarksR.Scripts.UI
{
    public class HorizontalSelection : MonoBehaviour
    {
        [SerializeField] private Button optionButton;
        public List<string> options;
        private int _selectedIndex;

        private void Start()
        {
            Select(0);
        }

        private void Select(int index)
        {
            if (index < 0 || index >= options.Count) return;
            _selectedIndex = index;
            optionButton.GetComponentInChildren<TMP_Text>().text = options[_selectedIndex];
        }

        public void SelectNext()
        {
            Select((_selectedIndex + 1) % options.Count);

        }

        public void SelectPrevious()
        {
            Select((_selectedIndex - 1 + options.Count) % options.Count);

        }

    }
}
