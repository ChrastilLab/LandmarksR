using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class NavigationTask : BaseTask
    {
        // private RepeatTask _parentRepeatTask;
        [NotEditable, SerializeField] private string target;


        protected override void Prepare()
        {
            base.Prepare();


            if (transform.parent.TryGetComponent<RepeatTask>(out var repeatTask))
            {
                var currentTrialData = repeatTask.table.Enumerator.GetCurrent();
                target = currentTrialData[0, "Target"] as string;

                Assert.IsNotNull(target, "NavigationTask target is null. Check your data table!");

                // _parentRepeatTask = repeatTask;
                // target = repeatTask.textTable.GetValue("Target");
            }
            else
            {
                logger.I("task", "Parent task is not a repeat task.");
            }


            hud.SetTitle("Navigation Task")
                .SetContent($"You will have {timer} seconds to find the target: {target}")
                .ShowAllComponents()
                .HideAllAfter(3f);

            playerController.TryEnableDesktopInput(3f);
            playerEvent.RegisterTriggerEnterHandler(HandlePlayerTriggerEnter);
        }

        private void HandlePlayerTriggerEnter(Collider other)
        {
            var obj = other.transform;
            if (!obj.CompareTag("Target")) return;

            isRunning = false;
        }

        protected override void Finish()
        {
            base.Finish();
            playerController.DisableDesktopInput();
            playerEvent.UnregisterTriggerEnterHandler(HandlePlayerTriggerEnter);
        }
    }
}
