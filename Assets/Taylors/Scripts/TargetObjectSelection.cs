using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Taylors.Scripts
{
    public class HighlightableButton
    {
        public HighlightableButton(Button button, Color originalColor)
        {
            this.button = button;
            this.originalColor = originalColor;
        }
        public readonly Button button;
        public readonly Color originalColor;
    }
    public class TargetObjectSelection : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private OVRInput.Button triggerButton = OVRInput.Button.PrimaryIndexTrigger;
        [SerializeField] private Transform buttonParentTransform;
        private Dictionary<string, HighlightableButton> buttonGroup;
        [CanBeNull] private Button highlightedButton;

        [SerializeField] private UnityEvent<string> targetSelected;

        private void Start()
        {
            Debug.Assert(buttonParentTransform != null, "Button Parent is not set");

            targetSelected.AddListener(Debug.Log);

            buttonGroup = new Dictionary<string, HighlightableButton>();

            foreach (Transform child in buttonParentTransform.transform)
            {
                if (child.TryGetComponent<Button>(out var button))
                {
                    Debug.Assert(buttonGroup.TryAdd(child.name, new HighlightableButton(button, button.image.color)), $"Button {child.name} already exists in the dictionary, make sure all buttons have unique names");
                    button.onClick.AddListener(() => { if (!highlightedButton) targetSelected?.Invoke(child.name); });
                }
            }
        }

        private void Update()
        {
            if (OVRInput.GetDown(triggerButton))
            {
                if (highlightedButton) targetSelected?.Invoke(highlightedButton.name);
            }
        }

        public void HighlightButton(string buttonName)
        {
            Debug.Log("Highlighting button: " + buttonName);
            // Set the button normal color to white
            foreach (var button in buttonGroup)
            {
                if (button.Key == buttonName)
                {
                    ApplyButtonAlpha(button.Value.button, 1f);
                    highlightedButton = button.Value.button;
                }
                else
                {
                    ApplyButtonAlpha(button.Value.button, 0.3f);
                }
            }
        }


        public void UnhighlightAllButtons()
        {
            foreach (var button in buttonGroup)
            {
                ApplyButtonAlpha(button.Value.button, button.Value.originalColor.a);
            }

            highlightedButton = null;
        }

        private static void ApplyButtonAlpha(Selectable button, float alpha)
        {
            var color = button.image.color;
            color.a = alpha;
            button.image.color = color;
        }
    }
}
