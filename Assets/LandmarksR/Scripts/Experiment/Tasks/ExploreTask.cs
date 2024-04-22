using LandmarksR.Scripts.Player;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ExploreTask : BaseTask
    {

        private HudMode _previousHudMode;
        protected override void Prepare()
        {
            base.Prepare();


            playerController.TryEnableDesktopInput();

            _previousHudMode = settings.displayReference.hudMode;
            settings.displayReference.hudMode = HudMode.Fixed;
            hud.ApplySettingChanges();

            hud.SetContent($"Explore the environment for {timer} seconds").SetTitle("");

            hud.SetOpacity(0.8f);
            hud.ShowByLayer("Environment");
        }

        protected override void Finish()
        {
            base.Finish();
            playerController.DisableDesktopInput();
            settings.displayReference.hudMode = _previousHudMode;
            hud.ApplySettingChanges();
            hud.SetOpacity(0);
        }


    }
}
