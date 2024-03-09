using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ExploreTask : BaseTask
    {
        private Hud _hud;
        private PlayerController _playerController;

        protected override void Prepare()
        {
            base.Prepare();

            _playerController = Experiment.Instance.playerController;
            _playerController.TryEnableDesktopInput();

            _hud = Experiment.Instance.playerController.hud;
            _hud.SetTitle($"Explore the environment for {timer} seconds")
                .ShowAll()
                .HideAllAfter(3f);


        }

        protected override void Finish()
        {
            base.Finish();
            _playerController.DisableDesktopInput();
        }


    }
}
