using System.Collections.Generic;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class InstructionTask : BaseTask
    {
        [SerializeField] private string instructionTitle;
        [TextArea(3, 10)]
        [SerializeField] private string instructionContent;

        [SerializeField] private float opacity = 0.5f;
        [SerializeField] private List<string> layersToHide = new();
        protected override void Prepare()
        {
            base.Prepare();

            HUD.SetTitle(instructionTitle)
                .SetContent(instructionContent)
                .ShowAll()
                .SetOpacity(opacity)
                .HideLayers(layersToHide);

            PlayerEvent.RegisterConfirmHandler(OnConfirm);
        }

        protected void UnregisterConfirmHandler()
        {
            PlayerEvent.UnregisterConfirmHandler(OnConfirm);
        }

        private void OnConfirm()
        {
            if (!isRunning) return;
            isRunning = false;
        }

        protected override void Finish()
        {
            base.Finish();
            PlayerEvent.UnregisterConfirmHandler(OnConfirm);

            HUD.HideAll()
                .ShowAllLayer();
        }
    }
}
