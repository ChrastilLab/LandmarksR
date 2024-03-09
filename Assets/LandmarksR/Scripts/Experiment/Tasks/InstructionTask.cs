using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class InstructionTask : BaseTask
    {
        private Hud _hud;
        private PlayerEventController _playerEventController;
        [SerializeField] private string instructionTitle;
        [SerializeField] private string instructionContent;
        protected override void Prepare()
        {
            base.Prepare();

            _hud = Experiment.Instance.playerController.hud;
            _hud.SwitchHudMode(HudMode.Follow)
                .SetTitle(instructionTitle)
                .SetContent(instructionContent)
                .ShowAll();

            _playerEventController = Experiment.Instance.playerController.playerEventController;
            _playerEventController.RegisterConfirmHandler(OnConfirm);
        }

        private void OnConfirm()
        {
            if (!isRunning) return;
            isRunning = false;
        }

        protected override void Finish()
        {
            base.Finish();
            _hud.HideAll();
            _playerEventController.UnregisterConfirmHandler(OnConfirm);
        }
    }
}
