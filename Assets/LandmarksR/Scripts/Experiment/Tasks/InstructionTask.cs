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

            hud.SetTitle(instructionTitle)
                .SetContent(instructionContent)
                .ShowAllComponents();

            hud.ShowAllLayer();
            foreach (var layer in layersToHide)
            {
                hud.HideByLayer(layer);
            }


            playerEvent.RegisterConfirmHandler(OnConfirm);
        }

        protected void UnregisterConfirmHandler()
        {
            playerEvent.UnregisterConfirmHandler(OnConfirm);
        }

        private void OnConfirm()
        {
            if (!isRunning) return;
            isRunning = false;
        }

        protected override void Finish()
        {
            base.Finish();
            hud.HideAll();
            playerEvent.UnregisterConfirmHandler(OnConfirm);

            foreach (var layer in layersToHide)
            {
                hud.ShowByLayer(layer);
            }
        }
    }
}
