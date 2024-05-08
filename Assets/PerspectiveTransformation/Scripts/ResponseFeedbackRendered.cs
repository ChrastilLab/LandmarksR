using LandmarksR.Scripts.Experiment.Data;
using LandmarksR.Scripts.Experiment.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace PerspectiveTransformation.Scripts
{
    public class ResponseFeedbackRendered : InstructionTask
    {
        [SerializeField] private Camera topDownCamera;
        [SerializeField] private Camera perspectiveCamera;
        [SerializeField] private RenderTexture topDownRenderTexture;
        [SerializeField] private RenderTexture perspectiveRenderTexture;
        [SerializeField] private GameObject feedbackPanel;
        [SerializeField] private RawImage leftImage;
        [SerializeField] private RawImage rightImage;
        [SerializeField] private TMP_Text text;
        [SerializeField] private GameObject arrow;


        protected override void Prepare()
        {
            Assert.IsNotNull(topDownCamera, $"({name}) Top Down Camera is not assigned");
            Assert.IsNotNull(perspectiveCamera, $"({name}) Perspective Camera is not assigned");
            Assert.IsNotNull(feedbackPanel, $"({name}) Feedback Panel is not assigned");
            Assert.IsNotNull(leftImage, $"({name}) Left Image is not assigned");
            Assert.IsNotNull(rightImage, $"({name}) Right Image is not assigned");
            Assert.IsNotNull(arrow, $"({name}) Arrow is not assigned");
            Assert.IsNotNull(text, $"({name}) Text is not assigned");


            var repeatTask = GetComponentInParent<RepeatTask>();
            Assert.IsNotNull(repeatTask, $"{name} must be a child of Repeat Task");

            base.Prepare();

            HUD.ShowAllLayer()
                .HideAll()
                .HideLayer("Environment")
                .HideLayer("Objects")
                .SetOpacity(0f)
                .SetTitle("Feedback");


            var camData = repeatTask.CurrentDataByTable(1);

            var isTopFirst = Utilities.GetOrderFromDataFrame(camData)[0] == 'T';
            if (isTopFirst)
            {
                SetFirstPersonView(perspectiveCamera, camData, 0);
                SetTopDownView(topDownCamera, 100);
                AdjustImageSize(leftImage, topDownRenderTexture, 900);
                AdjustImageSize(rightImage, perspectiveRenderTexture, 900);
            }
            else
            {
                SetFirstPersonView(perspectiveCamera, camData, 100);
                SetTopDownView(topDownCamera, 0);
                AdjustImageSize(leftImage, perspectiveRenderTexture, 900);
                AdjustImageSize(rightImage, topDownRenderTexture, 900);
            }


            var currentSpriteData = repeatTask.CurrentDataByTable(2);
            var correctness = !repeatTask.Context.ContainsKey("Correctness") || repeatTask.Context["Correctness"] == "0"
                ? "Your answer is Incorrect"
                : "Your answer is Correct";

            var feedbackText = correctness + "\n" +
                               currentSpriteData.GetFirstInColumn<string>("Feedback") +
                               "\n\nPress Space to continue";

            text.text = feedbackText;


            feedbackPanel.SetActive(true);
            arrow.SetActive(true);

            UnregisterDefaultKeyHandler(); // Unregister the default key handler in the InstructionTask base class
            PlayerEvent.RegisterKeyHandler(KeyCode.Space, Confirm);
        }

        protected override void Finish()
        {
            base.Finish();
            feedbackPanel.SetActive(false);
            arrow.SetActive(false);
        }

        private static void AdjustImageSize(RawImage image, Texture renderTexture, int imageWidth = 700)
        {
            image.texture = renderTexture;
            var ratio = (float)renderTexture.height / renderTexture.width;
            image.rectTransform.sizeDelta = new Vector2(imageWidth, imageWidth * ratio);
        }

        private static void SetTopDownView(Camera cam, float offset)
        {
            cam.transform.position = new Vector3(0, 100 + offset, -4f);
            cam.transform.rotation = Quaternion.Euler(90, 0, 0);
            cam.orthographic = true;
            cam.orthographicSize = 40;
        }

        private static void SetFirstPersonView(Camera cam, DataFrame camData, float offset)
        {
            var targetPosition = Utilities.GetPositionFromDataFrame(camData);
            var targetRotation = Utilities.GetRotationFromDataFrame(camData);

            targetPosition.y += offset;

            cam.transform.position = targetPosition;
            cam.transform.rotation = Quaternion.Euler(targetRotation);
            cam.orthographic = false;
        }

        private void Confirm()
        {
            if (!isRunning) return;
            isRunning = false;
        }
    }
}
