namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class ExploreTask : BaseTask
    {

        protected override void Prepare()
        {
            base.Prepare();

            playerController.TryEnableDesktopInput();

            hud.SetTitle($"Explore the environment for {timer} seconds")
                .ShowAll()
                .HideAllAfter(3f);


        }

        protected override void Finish()
        {
            base.Finish();
            playerController.DisableDesktopInput();
        }


    }
}
