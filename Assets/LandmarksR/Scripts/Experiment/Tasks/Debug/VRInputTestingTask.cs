﻿using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class VRInputTestingTask : TestingTask
    {
        protected override void Prepare()
        {
            base.Prepare();
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, () => Logger.I("input", "PrimaryIndexTrigger"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryHandTrigger, () => Logger.I("input", "PrimaryHandTrigger"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.One, () => Logger.I("input", "One/A"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.Two, () => Logger.I("input", "Two/B"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.Three, () => Logger.I("input", "Three/X"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.Four, () => Logger.I("input", "Four/Y"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.Start, () => Logger.I("input", "Start"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.Back, () => Logger.I("input", "Back"));
            Player.playerEvent.RegisterVRInputHandler(OVRInput.Button.PrimaryThumbstick, () => Logger.I("input", "PrimaryThumbstick"));

        }

        protected override void Finish()
        {
            base.Finish();
            Player.playerEvent.UnregisterAllVRInputHandlers();
        }


    }
}