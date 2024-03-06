using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ExploreTask : BaseTask
    {
        [SerializeField] private float timeout = 30f;
        private Hud _hud;
        private PlayerController _playerController;
        [NotEditable, SerializeField] private float remainingTime;

        protected override void Prepare()
        {
            base.Prepare();
            remainingTime = timeout;

            _playerController = Experiment.Instance.playerController;
            _playerController.EnableDesktopInput();

            _hud = Experiment.Instance.playerController.hud;
            _hud.ShowAll();
            _hud.ChangeTitle($"Explore the environment for {timeout} seconds");
            _hud.HideAllAfter(3f);
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

        protected override void Finish()
        {
            base.Finish();
            _playerController.DisableDesktopInput();
        }


    }
}
