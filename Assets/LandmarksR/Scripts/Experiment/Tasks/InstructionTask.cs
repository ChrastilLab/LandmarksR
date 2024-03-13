using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class InstructionTask : BaseTask
    {
        protected Hud Hud;
        protected PlayerEventController PlayerEvent;
        [SerializeField] private string instructionTitle;
        [TextArea(3, 10)]
        [SerializeField] private string instructionContent;
        protected override void Prepare()
        {
            base.Prepare();

            Hud = Experiment.Instance.playerController.hud;
            Hud.SetTitle(instructionTitle)
                .SetContent(instructionContent)
                .ShowAll();

            PlayerEvent = Experiment.Instance.playerController.playerEvent;
            PlayerEvent.RegisterConfirmHandler(OnConfirm);
        }

        private void OnConfirm()
        {
            if (!isRunning) return;
            isRunning = false;
        }

        protected override void Finish()
        {
            base.Finish();
            Hud.HideAll();
            PlayerEvent.UnregisterConfirmHandler(OnConfirm);
        }
    }
}
