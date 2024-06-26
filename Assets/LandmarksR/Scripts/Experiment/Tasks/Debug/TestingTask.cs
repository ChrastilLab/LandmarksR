﻿using System;
using System.Collections.Generic;
using LandmarksR.Scripts.Player;
using UnityEngine;
using LandmarksR.Scripts.Experiment;
using LandmarksR.Scripts.Experiment.Log;

namespace LandmarksR.Scripts.Experiment.Tasks.Debug
{
    public class TestingTask : BaseTask
    {
        private List<Action> _keyActions = new();

        private string _keyActionInstructions = "";

        protected override void Start()
        {
            base.Start();
            _keyActions = new List<Action>();
            AddKeyAction(KeyCode.Backspace, StopCurrentTask, "Stop Task");

        }

        protected void AddKeyAction(KeyCode code, Action action, string actionName = "")
        {
            _keyActionInstructions += $"{code} - {actionName}\n";
            _keyActions.Add(CreateKeyAction(code, action));
        }

        protected virtual void Update()
        {
            if (!IsTaskRunning()) return;
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
            if (!IsTaskRunning()) return;
            GUI.Label(new Rect(10, 10, 800, 800), _keyActionInstructions);
        }
    }
}
