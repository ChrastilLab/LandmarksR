using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{
    public class InstructionTask : Task
    {
        private Hud hud;
        protected override void Prepare()
        {
            base.Prepare();
            hud = Experiment.Instance.playerController.hud;
            hud.ChangeText("Press Enter to Proceed");
        }

        protected override void CleanUp()
        {
            base.CleanUp();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                IsCompleted = true;
            }
        }
    }
}