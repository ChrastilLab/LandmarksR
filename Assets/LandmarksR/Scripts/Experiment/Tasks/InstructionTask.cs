using LandmarksR.Scripts.Player;
using LandmarksR.Scripts.Utility;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class InstructionTask : BaseTask
    {
        private Hud _hud;
        [SerializeField] private string instruction;
        protected override void Prepare()
        {
            base.Prepare();
            _hud = Experiment.Instance.playerController.hud;
            _hud.ChangeTitle(instruction);
        }

        protected override void Finish()
        {
            base.Finish();
            _hud.HideTitle();
        }

        public void SetInstruction(string newInstruction)
        {
            instruction = newInstruction;
        }

        private void Update()
        {
            if (!isRunning) return;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                isRunning = false;
            }
        }
    }
}
