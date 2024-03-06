using System.Collections;
using LandmarksR.Scripts.Attributes;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class RepeatTask : BaseTask
    {
        [SerializeField] private int repeatCount;
        [NotEditable] public int currentCount;

        protected override void Prepare()
        {
            base.Prepare();
        }

        protected override void Finish()
        {
            base.Finish();
        }

        public override IEnumerator ExecuteAll()
        {
            while (currentCount < repeatCount)
            {
                yield return ExecuteSubTasks();
                ResetSubtasks();
                currentCount++;
            }
        }

        private IEnumerator ExecuteSubTasks()
        {
            foreach (var subTask in SubTasks)
            {
                yield return subTask.ExecuteAll();
            }
        }

        private void ResetSubtasks()
        {
            foreach (var task in SubTasks)
            {
                task.Reset();
            }
        }
    }
}
