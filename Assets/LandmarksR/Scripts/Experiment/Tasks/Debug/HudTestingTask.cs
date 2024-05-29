using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class HUDTestingTask: TestingTask
    {
        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();
            Player.TryEnableDesktopInput();
        }

        protected override void Start()
        {
            base.Start();
            AddKeyAction(KeyCode.Alpha1, ()=>HUD.ShowAll(), "Show All");
            AddKeyAction(KeyCode.Alpha2, ()=>HUD.HideAll(), "Hide All");
            AddKeyAction(KeyCode.Y, ()=>HUD.SwitchHudMode(HudMode.Follow), "Set HUD Mode Follow");
            AddKeyAction(KeyCode.F, ()=>HUD.SwitchHudMode(HudMode.Fixed), "Set HUD Mode Fixed");
            AddKeyAction(KeyCode.O, ()=>HUD.SwitchHudMode(HudMode.Overlay), "Set HUD Mode Overlay");
            AddKeyAction(KeyCode.Space, () => HUD.FixedRecenter(1.5f), "Recenter HUD");
        }

        public override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
        }
    }
}
