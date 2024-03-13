using System;
using System.Collections.Generic;
using LandmarksR.Scripts.Player;
using UnityEngine;
using LandmarksR.Scripts.Experiment;
using LandmarksR.Scripts.Experiment.Log;

namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class TestingTask : BaseTask
    {
        protected static Settings Settings => Settings.Instance;
        protected static Experiment Experiment => Experiment.Instance;
        protected static PlayerController Player => Experiment.Instance.playerController;
        protected static Hud Hud => Experiment.Instance.playerController.hud;

        protected static DebugLogger Logger => DebugLogger.Instance;

        private List<Action> _keyActions = new();

        private string _keyActionInstructions = "";

        protected override void Start()
        {
            base.Start();
            _keyActions = new List<Action>();
            AddKeyAction(KeyCode.Backspace, () => isRunning = false, "Stop Task");

        }

        protected void AddKeyAction(KeyCode code, Action action, string actionName = "")
        {
            _keyActionInstructions += $"{code} - {actionName}\n";
            _keyActions.Add(CreateKeyAction(code, action));
        }

        protected virtual void Update()
        {
            if (!isRunning) return;
            HandleInput();
        }

        private void HandleInput()
        {
            foreach (var action in _keyActions)
            {
                action?.Invoke();
            }
        }

        private static Action CreateKeyAction(KeyCode keyCode, Action action)
        {
            return () =>
            {
                if (Input.GetKeyDown(keyCode))
                {
                    action?.Invoke();
                }
            };
        }

        protected void OnGUI()
        {
            if (!isRunning) return;
            GUI.Label(new Rect(10, 10, 800, 800), _keyActionInstructions);
        }
    }
}
