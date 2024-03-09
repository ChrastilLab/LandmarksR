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
            Experiment.Instance.playerController.Teleport(position, rotation);
            isRunning = false;
        }

        protected override void Finish()
        {
            base.Finish();
        }
    }
}
