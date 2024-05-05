using LandmarksR.Scripts.Experiment.Data;
using LandmarksR.Scripts.Experiment.Log;
using LandmarksR.Scripts.Experiment.Tasks;
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

        private int _transformationStringIndex = 0;
        private bool isTopDown;

        private Camera _camera;

        [SerializeField] private bool isStaticLook;

        private RepeatTask repeatTask;
        private bool isFoil;


        protected override void Prepare()
        {
            base.Prepare();
            _camera = Player.GetMainCamera();

            if (isStaticLook)
            {
                _camera.transform.position = new Vector3(0, 100, -4f);
                _camera.transform.rotation = Quaternion.Euler(90, 0, 0);
                _camera.orthographic = true;
                _camera.orthographicSize = 40;
                return;
            }

            repeatTask = GetComponentInParent<RepeatTask>();
            Assert.IsNotNull(repeatTask, "Move Camera must be a child of Repeat Task");


            HUD.HideAll()
                .ShowAllLayer();



            _transformationStringIndex = isFirstShow ? 0 : 1;

            if (!isFirstShow)
            {
                responsePanel.SetActive(true);
                PlayerEvent.RegisterKeyHandler(KeyCode.F, HandleResponseYes);
                PlayerEvent.RegisterKeyHandler(KeyCode.J, HandleResponseNo);
            }

            var currentFoilData = repeatTask.CurrentDataByTable(0);
            var currentCameraData = repeatTask.CurrentDataByTable(1);

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

        private void HandleFirstPerson()
        {
            _targetPosition.y = 1.5f;
            _camera.transform.position = _targetPosition;
            _camera.transform.rotation = Quaternion.Euler(_targetRotation);
            _camera.orthographic = false;

            arrow.SetActive(false);

        }

        private void HandleTopDown()
        {
            _camera.transform.position = new Vector3(0, 100, -4f);
            _camera.transform.rotation = Quaternion.Euler(90, 0, 0);

            _camera.orthographic = true;
            _camera.orthographicSize = 40;

            arrow.SetActive(true);
            arrow.transform.position = _targetPosition;
            arrow.transform.rotation = Quaternion.Euler(_targetRotation);
        }

        private void HandleResponseYes()
        {
            isRunning = false;
            repeatTask.Context["Correctness"] = isFoil ? "1" : "0";
            Logger.I("response", "Same");
        }

        private void HandleResponseNo()
        {
            isRunning = false;
            repeatTask.Context["Correctness"] = isFoil ? "0" : "1";
            Logger.I("response", "Different");
        }

        protected override void Finish()
        {
            base.Finish();
            // Prevent flickering by pre-setting the position and rotation
            if (isTopDown)
                HandleFirstPerson();
            else
                HandleTopDown();

            if (!isFirstShow && !isStaticLook)
            {
                responsePanel.SetActive(false);
                PlayerEvent.UnregisterKeyHandler(KeyCode.F, HandleResponseYes);
                PlayerEvent.UnregisterKeyHandler(KeyCode.J, HandleResponseNo);
            }
        }
    }
}
