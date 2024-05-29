using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    /// <summary>
    /// Represents a task that teleports the player to a specified position and rotation.
    /// </summary>
    public class TeleportTask : BaseTask
    {
        /// <summary>
        /// The position to which the player will be teleported.
        /// </summary>
        [SerializeField] private Vector3 position;

        /// <summary>
        /// The rotation to which the player will be teleported.
        /// </summary>
        [SerializeField] private Vector3 rotation;

        /// <summary>
        /// Prepares the teleport task by teleporting the player to the specified position and rotation.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Functional);
            base.Prepare();
            Player.Teleport(position, rotation);
        }
    }
}
