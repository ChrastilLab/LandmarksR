using System.Collections;
using System.Collections.Generic;
using LandmarksR.Scripts.Experiment.Tasks;
using LandmarksR.Scripts.Experiment.Tasks.Interactive;
using LandmarksR.Scripts.Experiment.Tasks.Structural;
using UnityEngine;
using UnityEngine.Assertions;

namespace PerspectiveTransformation.Scripts
{
    public class BreakTime : InstructionTask
    {
        [SerializeField] private List<int> breakPoints;
        private bool confirmBreak = false;

        protected override void Prepare()
        {

            var repeatTask = GetComponentInParent<RepeatTask>();
            Assert.IsNotNull(repeatTask, $"{name} must be a child of Repeat Task");

            SetTaskType(TaskType.Interactive);
            base.Prepare();

            // check if repeatTask.currentRepeat is in breakPoints
            if (!breakPoints.Contains(repeatTask.currentRepeat))
            {
                StopCurrentTask();
                return;
            }

            UnregisterDefaultKeyHandler();
            PlayerEvent.RegisterKeyHandler(KeyCode.Space, HandleSpace);
        }

        private void HandleSpace()
        {
            if (!IsTaskRunning()) return;

            if (!confirmBreak)
            {
                confirmBreak = true;
                StartCoroutine(ConfirmTimeout());
                return;
            }

            StopCurrentTask();
        }

        private IEnumerator ConfirmTimeout()
        {
            yield return new WaitForSeconds(0.2f);
            confirmBreak = false;
        }

        public override void Finish()
        {
            base.Finish();
            PlayerEvent.UnregisterKeyHandler(KeyCode.Space, HandleSpace);
        }
    }
}
