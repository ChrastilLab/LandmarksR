using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    /// <summary>
    /// Represents an explore task where the player can freely explore the environment.
    /// </summary>
    public class ExploreTask : InstructionTask
    {
        /// <summary>
        /// The time after which the instruction is hidden.
        /// </summary>
        [SerializeField] private float hideInstructionAfter = 3;

        /// <summary>
        /// The key to skip the explore task.
        /// </summary>
        [SerializeField] private KeyCode skipKey = KeyCode.Backspace;

        /// <summary>
        /// Prepares the explore task, enabling player input and setting up the HUD.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();
            UnregisterDefaultKeyHandler();

            Player.TryEnableDesktopInput();
            Player.StartPlayerLogging();

            Player.GetMainCamera().orthographic = false;
            HUD.HideAllAfter(timer <= hideInstructionAfter ? timer - 0.5f : hideInstructionAfter); // Ensure the instruction is hidden before the task ends
            PlayerEvent.RegisterKeyHandler(skipKey, Skip);
        }

        /// <summary>
        /// Finishes the explore task, disabling player input and clearing the HUD.
        /// </summary>
        public override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
            Player.StopPlayerLogging();
            PlayerEvent.UnregisterKeyHandler(KeyCode.Backspace, Skip);
            HUD.ClearAllText();
        }

        /// <summary>
        /// Skips the explore task when the backspace key is pressed.
        /// </summary>
        private void Skip()
        {
            StopCurrentTask();
        }
    }
}
