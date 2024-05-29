using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    /// <summary>
    /// Represents a task that displays instructions to the player.
    /// </summary>
    public class InstructionTask : BaseTask
    {
        /// <summary>
        /// The title of the instruction.
        /// </summary>
        [SerializeField] private string instructionTitle;

        /// <summary>
        /// The content of the instruction.
        /// </summary>
        [TextArea(3, 10)]
        [SerializeField] private string instructionContent;

        /// <summary>
        /// The alignment of the instruction text.
        /// </summary>
        [SerializeField] private TextAlignmentOptions textAlignmentOptions = TextAlignmentOptions.TopRight;

        /// <summary>
        /// The opacity of the instruction text.
        /// </summary>
        [SerializeField] private float opacity = 0.5f;

        /// <summary>
        /// The layers to hide while displaying the instruction.
        /// </summary>
        [SerializeField] private List<string> layersToHide = new();

        /// <summary>
        /// Prepares the instruction task by setting up the HUD and registering event handlers.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();

            // Set up the HUD with the instruction details.
            HUD.SetTitle(instructionTitle)
                .SetContent(instructionContent)
                .ShowAll()
                .SetOpacity(opacity)
                .HideLayers(layersToHide)
                .SetContentAlignment(textAlignmentOptions);

            // Register the confirm handler to allow the player to confirm the instruction.
            PlayerEvent.RegisterConfirmHandler(OnConfirm);
        }

        /// <summary>
        /// Unregisters the default key handler for confirming instructions.
        /// </summary>
        protected void UnregisterDefaultKeyHandler() => UnregisterConfirmHandler();

        /// <summary>
        /// Unregisters the confirmation handler.
        /// </summary>
        protected void UnregisterConfirmHandler()
        {
            PlayerEvent.UnregisterConfirmHandler(OnConfirm);
        }

        /// <summary>
        /// Handles the confirmation event when the player confirms the instruction.
        /// </summary>
        private void OnConfirm()
        {
            StopCurrentTask();
        }

        /// <summary>
        /// Finishes the instruction task by clearing the HUD and unregistering event handlers.
        /// </summary>
        public override void Finish()
        {
            base.Finish();
            PlayerEvent.UnregisterConfirmHandler(OnConfirm);
            HUD.ClearAllText();
        }
    }
}
