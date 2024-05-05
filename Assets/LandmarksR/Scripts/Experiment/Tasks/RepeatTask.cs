using System;
using System.Collections;
using System.Collections.Generic;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Data;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    [Serializable]
    public class RepeatOption
    {
        public bool useTable;
        public Table table;
        public int numberOfRepeat = 3;
    }

    public class RepeatTask : BaseTask
    {
        [Header("Repeat Task Settings")]

        [SerializeField] private RepeatOption repeatOption;

        [Tooltip("Current SubTask Number (0-indexed)")]
        [NotEditable] public int currentRepeat = 1;

        [Tooltip("Current SubTask Number (1-indexed)")]
        [NotEditable] public int currentSubTaskNumber = 1;

        [SerializeField] private bool showDebug = true;

        public Dictionary<string, string> Context = new();
        public Table CurrentTable => repeatOption.table;
        public DataFrame CurrentData => repeatOption.table.Enumerator.GetCurrent();
        public DataFrame CurrentDataByTable(int tableIndex) => repeatOption.table.Enumerator.GetCurrentByTable(tableIndex);
        private delegate IEnumerator ExecuteAllDelegate();
        private ExecuteAllDelegate _executeAll;




        protected override void Prepare()
        {
            base.Prepare();
            if (repeatOption.useTable && repeatOption.table )
            {
                repeatOption.numberOfRepeat = repeatOption.table.Count;
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
            if (repeatOption.table.Enumerator == null)
            {
                Logger.E("Repeat Task", "Enumerator is null. Check if the table is initialized and enabled.");
                yield break;
            }

            isSubTaskRunning = true;
            while (repeatOption.table.Enumerator.MoveNext())
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
            while (currentRepeat <= repeatOption.numberOfRepeat)
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

            Context.Clear();
        }

        private void OnGUI()
        {
            if (!isSubTaskRunning || !showDebug) return;
            GUI.Label(new Rect(10, 10, 100, 20), $"Repeat: {currentRepeat}/{repeatOption.numberOfRepeat}");
            GUI.Label(new Rect( 10, 30, 100, 20), $"SubTask: {currentSubTaskNumber}/{_subTasks.Count}");
        }
    }
}
