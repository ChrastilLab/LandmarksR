using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Taylors.Scripts
{
    [Serializable]
    public class TargetInfo
    {
        public string targetName;
        public Sprite sprite;
    }

    [Serializable]
    public class HighlightableButton
    {
        public Button button;
        public Collider collider;
        public Color originalColor;
    }

    public class TargetObjectSelection : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private OVRInput.Button triggerButton = OVRInput.Button.PrimaryIndexTrigger;
        [SerializeField] private List<HighlightableButton> buttons = new();
        [CanBeNull] private TargetInfo _highlightedInfo;

        [SerializeField] private List<TargetInfo> targetInfos = new();

        private List<Tuple<TargetInfo, HighlightableButton>> _targetButtonPairs = new();
        [SerializeField] private bool randomizeButtonOrder = false;
        [SerializeField] private float randomizingSeed = 1024;

        public UnityEvent<string> targetSelected;
        private void Start()
        {
            foreach (var button in buttons)
            {
                button.originalColor = button.button.image.color;
            }

            // Order by sibling index
            buttons = buttons.OrderBy(a => a.button.transform.GetSiblingIndex()).ToList();

            Assert.IsTrue(targetInfos.Count > 0, "Target Buttons are not set");
            Assert.AreEqual(targetInfos.Count, buttons.Count, "Target Infos and Buttons are not the same size");

            // Randomize the order of the target info that's gonna be mapped to the buttons
            if (randomizeButtonOrder)
            {
                var random = new System.Random((int) randomizingSeed);
                targetInfos = targetInfos.OrderBy(a => random.Next()).ToList();
            }

            // Map the target info to the buttons
            _targetButtonPairs = targetInfos
                .Zip(buttons, (info, button) => new Tuple<TargetInfo, HighlightableButton>(info, button)).ToList();

            foreach (var pair in _targetButtonPairs)
            {
                pair.Item2.button.image.sprite = pair.Item1.sprite;
                pair.Item2.button.onClick.AddListener(() =>
                {
                    if (_highlightedInfo == null) targetSelected?.Invoke(pair.Item1.targetName);
                });
            }
        }

        private void Update()
        {
            if (OVRInput.GetDown(triggerButton))
            {
                if (_highlightedInfo != null) targetSelected?.Invoke(_highlightedInfo.targetName);
            }
        }

        public void HighlightButton(Collider buttonCollider)
        {
            foreach (var pair in _targetButtonPairs)
            {
                if (pair.Item2.collider == buttonCollider)
                {
                    _highlightedInfo = pair.Item1;
                    ApplyButtonAlpha(pair.Item2.button, pair.Item2.originalColor.a);
                }
                else
                {
                    ApplyButtonAlpha(pair.Item2.button, 0.3f);
                }
            }
        }


        public void UnhighlightAllButtons()
        {
            foreach (var pair in _targetButtonPairs)
            {
                ApplyButtonAlpha(pair.Item2.button, pair.Item2.originalColor.a);
            }

            _highlightedInfo = null;
        }

        private static void ApplyButtonAlpha(Selectable button, float alpha)
        {
            var color = button.image.color;
            color.a = alpha;
            button.image.color = color;
        }
    }
}
