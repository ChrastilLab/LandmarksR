using System.Collections.Generic;
using LandmarksR.Scripts.Experiment.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace PerspectiveTransformation.Scripts
{
    public class ResponseFeedback : InstructionTask
    {
        [SerializeField] private GameObject responsePanel;
        [SerializeField] private Image image1;
        [SerializeField] private Image image2;
        [SerializeField] private TMP_Text text;

        protected override void Prepare()
        {
            Assert.IsNotNull(responsePanel, $"({name}) Response Panel is not assigned");
            Assert.IsNotNull(image1, $"({name}) Image1 is not assigned");
            Assert.IsNotNull(image2, $"({name}) Image2 is not assigned");

            var repeatTask = GetComponentInParent<RepeatTask>();
            Assert.IsNotNull(repeatTask, $"{name} must be a child of Repeat Task");

            base.Prepare();
            HUD.ShowAllLayer()
                .HideAll()
                .HideLayer("Environment")
                .HideLayer("Objects")
                .SetOpacity(0f)
                .SetTitle("Feedback");


            var currentSpriteData = repeatTask.CurrentDataByTable(2);

            var spriteName1 = currentSpriteData.GetFirstInColumn<string>("Sprite1");
            var spriteName2 = currentSpriteData.GetFirstInColumn<string>("Sprite2");

            var sprite1 = Resources.Load<Sprite>("Practice Screenshots/" + spriteName1);
            var sprite2 = Resources.Load<Sprite>("Practice Screenshots/" + spriteName2);

            Assert.IsNotNull(sprite1, $"({name}) Sprite1 {spriteName1} not found");
            Assert.IsNotNull(sprite2, $"({name}) Sprite2 {spriteName2} not found");

            image1.sprite = sprite1;
            image2.sprite = sprite2;

            var correctness = !repeatTask.Context.ContainsKey("Correctness") || repeatTask.Context["Correctness"] == "0"
                ? "Your answer is Incorrect"
                : "Your answer is Correct";

            var feedbackText = correctness + "\n" +
                               currentSpriteData.GetFirstInColumn<string>("Feedback") +
                               "\n\nPress Space to continue";

            text.text = feedbackText;

            responsePanel.SetActive(true);

            PlayerEvent.RegisterKeyHandler(KeyCode.Space, Confirm);
        }

        protected override void Finish()
        {
            base.Finish();
            responsePanel.SetActive(false);
            HUD.ShowAllLayer().HideAll();
        }

        private void Confirm()
        {
            if (!isRunning) return;
            isRunning = false;
        }
    }
}
