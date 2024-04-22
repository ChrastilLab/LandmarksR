using LandmarksR.Scripts.Experiment.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace PerspectiveTransformation.Scripts
{
    public class ShowCamera : BaseTask
    {
        public Vector3 TargetPosition { get; private set; }
        public Vector3 TargetRotation { get; private set; }

        [SerializeField] private GameObject arrow;
        [SerializeField] private bool isFirstShow = true;
        [SerializeField] private GameObject responsePanel;

        private int _transformationStringIndex = 0;
        private bool isTopDown;

        private Camera _camera;


        protected override void Prepare()
        {
            base.Prepare();

            var repeatTask = GetComponentInParent<RepeatTask>();
            Assert.IsNotNull(repeatTask, "Move Camera must be a child of Repeat Task");

            var table = repeatTask.table;
            Assert.IsNotNull(table, "Table must be assigned to Repeat Task");



            _camera = playerController.GetMainCamera();

            _transformationStringIndex = isFirstShow ? 0 : 1;

            if (!isFirstShow)
            {
                responsePanel.SetActive(true);
                playerEvent.RegisterKeyHandler(KeyCode.F, HandleResponseYes);
                playerEvent.RegisterKeyHandler(KeyCode.J, HandleResponseNo);
            }

            var current = table.Enumerator.GetCurrent();

            TargetPosition = Utilities.GetPositionFromDataFrame(current);
            TargetRotation = Utilities.GetRotationFromDataFrame(current);
            isTopDown = Utilities.GetOrderFromDataFrame(current)[_transformationStringIndex] == 'T';

            if (isTopDown)
                HandleTopDown();
            else
                HandleFirstPerson();
        }

        private void HandleFirstPerson()
        {
            _camera.transform.position = TargetPosition;
            _camera.transform.rotation = Quaternion.Euler(TargetRotation);
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
            arrow.transform.position = TargetPosition;
            arrow.transform.rotation = Quaternion.Euler(TargetRotation);
        }

        private void HandleResponseYes()
        {

        }

        private void HandleResponseNo()
        {

        }

        protected override void Finish()
        {
            base.Finish();
            if (!isFirstShow)
            {
                responsePanel.SetActive(false);
                playerEvent.UnregisterKeyHandler(KeyCode.F, HandleResponseYes);
                playerEvent.UnregisterKeyHandler(KeyCode.J, HandleResponseNo);
            }
        }
    }
}
