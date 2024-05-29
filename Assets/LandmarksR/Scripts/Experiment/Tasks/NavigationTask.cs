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
        [SerializeField] private string targetNameColumn = "Target";
        [SerializeField] private string targetObjectTag = "Target";


        protected override void Prepare()
        {
            SetTaskType(TaskType.Interactive);
            base.Prepare();

            if (transform.parent.TryGetComponent<RepeatTask>(out var repeatTask))
            {
                var currentTrialData = repeatTask.CurrentData;
                target = currentTrialData.GetFirstInColumn<string>("Target");

                Assert.IsNotNull(target, "NavigationTask target is null. Check your data table!");

                // _parentRepeatTask = repeatTask;
                // target = repeatTask.textTable.GetValue("Target");
            }
            else
            {
                Logger.I("task", "Parent task is not a repeat task.");
            }


            HUD.SetTitle("Navigation Task")
                .SetContent($"You will have {timer} seconds to find the target: {target}")
                .ShowAll()
                .HideAllAfter(3f);

            Player.TryEnableDesktopInput(3f);
            PlayerEvent.RegisterTriggerEnterHandler(HandlePlayerTriggerEnter);
        }

        private void HandlePlayerTriggerEnter(Collider other)
        {
            var obj = other.transform;
            if (!obj.CompareTag(targetObjectTag)) return;

            StopCurrentTask();
        }

        public override void Finish()
        {
            base.Finish();
            Player.DisableDesktopInput();
            PlayerEvent.UnregisterTriggerEnterHandler(HandlePlayerTriggerEnter);
        }
    }
}
