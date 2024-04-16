using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class TeleportTask : BaseTask
    {
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;
        protected override void Prepare()
        {
            base.Prepare();
            playerController.Teleport(position, rotation);
            isRunning = false;
        }
    }
}
