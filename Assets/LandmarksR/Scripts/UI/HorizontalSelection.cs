using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LandmarksR.Scripts.UI
{
    public class HorizontalSelection : MonoBehaviour
    {
        [SerializeField] private TMP_Text optionText;
        private List<string> _options;
        private int _selectedIndex;

        private void Start()
        {
            // Select(0);
        }

        public void SetList(IEnumerable<string> list)
        {
            _options = new List<string>(list);
            Select(0);
        }

        private void Select(int index)
        {
            if (index < 0 || index >= _options.Count) return;
            _selectedIndex = index;
            optionText.text = _options[_selectedIndex];
        }

        public void SelectNext()
        {
            Select((_selectedIndex + 1) % _options.Count);

        }

        public void SelectPrevious()
        {
            Select((_selectedIndex - 1 + _options.Count) % _options.Count);

        }

        public string GetSelectedOption()
        {
            return _options[_selectedIndex];
        }

    }
}
