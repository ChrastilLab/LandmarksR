using LandmarksR.Scripts.Player;
using LandmarksR.Scripts.Utility;

namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class HudTestingTask: TestingTask
    {
        protected override void Prepare()
        {
            base.Prepare();
            Player.EnableDesktopInput();
        }

        protected override void Alpha1()
        {
            DebugLogger.Instance.Log("Show hud", "tests");
            Hud.ShowAll();
        }

        protected override void Alpha2()
        {
            DebugLogger.Instance.Log("Hide hud", "tests");
            Hud.HideAll();
        }

        protected override void Alpha3()
        {
            DebugLogger.Instance.Log("Switch to follow mode", "tests");
            Config.UpdateHudMode(HudMode.Follow, Hud);
        }

        protected override void Alpha4()
        {
            DebugLogger.Instance.Log("Switch to fixed mode", "tests");
            Config.UpdateHudMode(HudMode.Fixed, Hud);
        }

        protected override void Alpha5()
        {
            DebugLogger.Instance.Log("Switch to overlay mode", "tests");
            Config.UpdateHudMode(HudMode.Overlay, Hud);
        }

        protected override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
        }
    }
}
