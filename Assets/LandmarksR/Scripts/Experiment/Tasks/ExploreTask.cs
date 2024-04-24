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

            HUD.HideAllAfter(timer <= hideInstructionAfter ? timer - 0.5f : hideInstructionAfter); // ensure that the instruction is hidden before the task ends
        }

        protected override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
            HUD.ClearAllText()
                .ShowAllLayer();
        }
    }
}
