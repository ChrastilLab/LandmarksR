using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class HudTestingTask: TestingTask
    {
        protected override void Prepare()
        {
            base.Prepare();
            Player.TryEnableDesktopInput();
        }

        protected override void Start()
        {
            base.Start();
            AddKeyAction(KeyCode.Alpha1, ()=>Hud.ShowAllComponents(), "Show All");
            AddKeyAction(KeyCode.Alpha2, ()=>Hud.HideAll(), "Hide All");
            AddKeyAction(KeyCode.Y, ()=>Hud.SwitchHudMode(HudMode.Follow), "Set Hud Mode Follow");
            AddKeyAction(KeyCode.F, ()=>Hud.SwitchHudMode(HudMode.Fixed), "Set Hud Mode Fixed");
            AddKeyAction(KeyCode.O, ()=>Hud.SwitchHudMode(HudMode.Overlay), "Set Hud Mode Overlay");
            AddKeyAction(KeyCode.Space, () => Hud.FixedRecenter(1.5f), "Recenter Hud");
        }

        protected override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
        }
    }
}
