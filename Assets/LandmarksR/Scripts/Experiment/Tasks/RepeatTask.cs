using System.Collections;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Data;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class RepeatTask : BaseTask
    {
        [Tooltip("This overrides the number of repeat")]
        [SerializeField] private bool useTable;
        [SerializeField] public Table table;
        private IEnumerator _enumerator;
        [SerializeField] private int numberOfRepeat = 3;

        [Tooltip("Current SubTask Number (0-indexed)")]
        [NotEditable] public int currentRepeat = 1;

        [Tooltip("Current SubTask Number (1-indexed)")]
        [NotEditable] public int currentSubTaskNumber = 1;

        private delegate IEnumerator ExecuteAllDelegate();
        private ExecuteAllDelegate _executeAll;


        protected override void Prepare()
        {
            base.Prepare();
            if (useTable && table )
            {
                numberOfRepeat = table.Count;
                _executeAll = ExecuteByTable;
            }
            else
            {
                _executeAll = ExecuteByRepeat;
            }
        }

        // protected override void Finish()
        // {
        //     base.Finish();
        // }

        private IEnumerator ExecuteByTable()
        {
            isSubTaskRunning = true;
            while (table.Enumerator.MoveNext())
            {
                yield return ExecuteSubTasks();
                ResetSubtasks();
                currentRepeat++;
            }
            isSubTaskRunning = false;
        }

        private IEnumerator ExecuteByRepeat()
        {
            isSubTaskRunning = true;
            while (currentRepeat <= numberOfRepeat)
            {
                yield return ExecuteSubTasks();
                ResetSubtasks();
                currentRepeat++;
            }
            isSubTaskRunning = false;
        }

        public override IEnumerator ExecuteAll()
        {
            if (!enable) yield break;

            Prepare();

            isSubTaskRunning = true;
            yield return _executeAll?.Invoke();
            isSubTaskRunning = false;

            Finish();
        }



        private IEnumerator ExecuteSubTasks()
        {
            foreach (var subTask in _subTasks)
            {
                yield return subTask.ExecuteAll();
                currentSubTaskNumber++;
            }
        }

        private void ResetSubtasks()
        {
            currentSubTaskNumber = 1;
            foreach (var task in _subTasks)
            {
                task.Reset();
            }
        }

        private void OnGUI()
        {
            if (!isSubTaskRunning) return;
            GUI.Label(new Rect(10, 10, 100, 20), $"Repeat: {currentRepeat}/{numberOfRepeat}");
            GUI.Label(new Rect( 10, 30, 100, 20), $"SubTask: {currentSubTaskNumber}/{_subTasks.Count}");
        }
    }
}
