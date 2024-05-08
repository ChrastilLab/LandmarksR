using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ExploreTask : InstructionTask
    {
        [SerializeField] private float hideInstructionAfter = 3;
        protected override void Prepare()
        {
            base.Prepare();

            Player.TryEnableDesktopInput();
            Player.StartPlayerLogging();

            HUD.HideAllAfter(timer <= hideInstructionAfter ? timer - 0.5f : hideInstructionAfter); // ensure that the instruction is hidden before the task ends
            PlayerEvent.RegisterKeyHandler(KeyCode.Backspace, Skip);
        }

        protected override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
            Player.StopPlayerLogging();
            PlayerEvent.UnregisterKeyHandler(KeyCode.Backspace, Skip);
            HUD.ClearAllText()
                .ShowAllLayer();

        }

        private void Skip()
        {
            if (!isRunning) return;
            isRunning = false;
        }
    }
}
