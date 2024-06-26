﻿using LandmarksR.Scripts.Experiment.Tasks;
using LandmarksR.Scripts.Experiment.Tasks.Structural;
using UnityEngine;
using UnityEngine.Assertions;

namespace PerspectiveTransformation.Scripts
{
    public class ShowCamera : BaseTask
    {
        private Vector3 _targetPosition;
        private Vector3 _targetRotation;

        [SerializeField] private GameObject arrow;
        [SerializeField] private bool isFirstShow = true;
        [SerializeField] private GameObject responsePanel;

        private int _transformationStringIndex;
        private bool isTopDown;

        private Camera _camera;

        [SerializeField] private bool isStaticLook;

        private RepeatTask repeatTask;
        private bool isFoil;


        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();
            _camera = Player.GetMainCamera();

            HUD.HideAll()
                .ShowAllLayer();

            PlayerEvent.RegisterKeyHandler(KeyCode.Backspace, Skip);

            if (isStaticLook)
            {
                _camera.transform.position = new Vector3(0, 100, -4f);
                _camera.transform.rotation = Quaternion.Euler(90, 0, 0);
                _camera.orthographic = true;
                _camera.orthographicSize = 40;
                return; // Careful with this return statement, it will skip the rest of the code
            }

            repeatTask = GetComponentInParent<RepeatTask>();
            Assert.IsNotNull(repeatTask, "Move Camera must be a child of Repeat Task");





            _transformationStringIndex = isFirstShow ? 0 : 1;

            if (!isFirstShow)
            {
                responsePanel.SetActive(true);
                PlayerEvent.RegisterKeyHandler(KeyCode.F, HandleResponseYes);
                PlayerEvent.RegisterKeyHandler(KeyCode.J, HandleResponseNo);
            }

            var currentFoilData = repeatTask.CurrentDataByTable(0);
            var currentCameraData = repeatTask.CurrentDataByTable(1);

            repeatTask.Context.TryAdd("Type", currentFoilData.GetFirstInColumn<string>("Type"));
            repeatTask.Context.TryAdd("Arg1", $"\"{currentFoilData.GetFirstInColumn<string>("Arg1")}\"");
            repeatTask.Context.TryAdd("Arg2", $"\"{currentFoilData.GetFirstInColumn<string>("Arg2")}\"");
            repeatTask.Context.TryAdd("PX", currentCameraData.GetFirstInColumn<string>("PX"));
            repeatTask.Context.TryAdd("PY", currentCameraData.GetFirstInColumn<string>("PY"));
            repeatTask.Context.TryAdd("PZ", currentCameraData.GetFirstInColumn<string>("PZ"));
            repeatTask.Context.TryAdd("RX", currentCameraData.GetFirstInColumn<string>("RX"));
            repeatTask.Context.TryAdd("RY", currentCameraData.GetFirstInColumn<string>("RY"));
            repeatTask.Context.TryAdd("RZ", currentCameraData.GetFirstInColumn<string>("RZ"));
            repeatTask.Context.TryAdd("ORDER", currentCameraData.GetFirstInColumn<string>("ORDER"));



            isFoil = currentFoilData.GetFirstInColumn<string>("Type").Equals("No Foil");

            _targetPosition = Utilities.GetPositionFromDataFrame(currentCameraData);
            _targetRotation = Utilities.GetRotationFromDataFrame(currentCameraData);

            isTopDown = Utilities.GetOrderFromDataFrame(currentCameraData)[_transformationStringIndex] == 'T';

            if (isTopDown)
            {
                _targetRotation = FoilControl.ApplyFoilDirectionToArrow(currentFoilData, _targetRotation); // Apply foil direction conditionally
                HandleTopDown();
            }
            else
                HandleFirstPerson();

            repeatTask.Context.Remove("Correctness");


        }

        private void HandleFirstPerson(bool showArrow=false)
        {
            _targetPosition.y = 1.5f;
            _camera.transform.position = _targetPosition;
            _camera.transform.rotation = Quaternion.Euler(_targetRotation);
            _camera.orthographic = false;

            arrow.SetActive(showArrow);

        }

        private void HandleTopDown(bool showArrow=true)
        {
            _camera.transform.position = new Vector3(0, 100, -4f);
            _camera.transform.rotation = Quaternion.Euler(90, 0, 0);

            _camera.orthographic = true;
            _camera.orthographicSize = 40;

            arrow.SetActive(showArrow);
            arrow.transform.position = _targetPosition;
            arrow.transform.rotation = Quaternion.Euler(_targetRotation);
        }

        private void HandleResponseYes()
        {
            StopCurrentTask();
            repeatTask.Context["Response"] = "1";
            repeatTask.Context["Correctness"] = isFoil ? "1" : "0";
            repeatTask.Context["ReactionTime"] = $"{elapsedTime}";
            Logger.I("response", "Same");
        }

        private void HandleResponseNo()
        {
            StopCurrentTask();
            repeatTask.Context["Correctness"] = isFoil ? "0" : "1";
            repeatTask.Context["Response"] = "0";
            repeatTask.Context["ReactionTime"] = $"{elapsedTime}";
            Logger.I("response", "Different");
        }

        private void Skip()
        {
            StopCurrentTask();
        }

        public override void Finish()
        {
            base.Finish();
            HUD.HideLayers(new[] { "Objects", "Environment" });
            // Prevent flickering by pre-setting the position and rotation

            PlayerEvent.UnregisterKeyHandler(KeyCode.Backspace, Skip);
            if (isStaticLook)
            {
                return;
            }


            // if (isTopDown)
            //     HandleFirstPerson(false);
            // else
            //     HandleTopDown(false);

            Player.GetMainCamera().transform.localPosition = new Vector3(0, 1.7f, 0);

            if (!isFirstShow )
            {
                responsePanel.SetActive(false);
                PlayerEvent.UnregisterKeyHandler(KeyCode.F, HandleResponseYes);
                PlayerEvent.UnregisterKeyHandler(KeyCode.J, HandleResponseNo);
                var timeout = elapsedTime >= timer ? "1" : "0";
                repeatTask?.Context.TryAdd("Timeout", $"{timeout}");

                var reactionTime = elapsedTime;
                repeatTask?.Context.TryAdd("ReactionTime", $"{reactionTime}");
            }

        }
    }
}
