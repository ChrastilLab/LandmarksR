using LandmarksR.Scripts.Player;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ExploreTask : BaseTask
    {

        protected override void Prepare()
        {
            base.Prepare();


            playerController.TryEnableDesktopInput();

            settings.displayReference.hudMode = HudMode.Fixed;
            hud.ApplySettingChanges();

            hud.SetTitle($"Explore the environment for {timer} seconds")
                .SetContent("Other Instruction here")
                .ShowButton()
                .ShowAllComponents();


        }

        protected override void Finish()
        {
            base.Finish();
            playerController.DisableDesktopInput();
        }


    }
}
