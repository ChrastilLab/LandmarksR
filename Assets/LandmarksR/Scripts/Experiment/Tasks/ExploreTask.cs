using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ExploreTask : BaseTask
    {
        [SerializeField] private float timeout = 30f;
        private Hud _hud;
        [NotEditable, SerializeField] private float remainingTime;

        protected override void Prepare()
        {
            base.Prepare();
            remainingTime = timeout;

            _hud = Experiment.Instance.playerController.hud;
            _hud.ChangeText($"Explore the environment for {timeout} seconds");
            _hud.HidePanelAfter(3f);
        }

        private void Update()
        {
            if (!isRunning) return;

            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                isRunning = false;
            }

        }


    }
}
