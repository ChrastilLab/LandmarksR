using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class InstructionTask : BaseTask
    {
        [SerializeField] private string instructionTitle;
        [TextArea(3, 10)]
        [SerializeField] private string instructionContent;
        protected override void Prepare()
        {
            base.Prepare();

            hud.SetTitle(instructionTitle)
                .SetContent(instructionContent)
                .ShowAll();

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
        }
    }
}
