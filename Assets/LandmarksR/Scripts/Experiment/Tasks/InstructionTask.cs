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
            _hud.ChangeText(instruction);
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _hud.HideText();
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
