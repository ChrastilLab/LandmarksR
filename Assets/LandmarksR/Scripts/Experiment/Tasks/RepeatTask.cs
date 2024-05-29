using System;
using System.Collections;
using System.Collections.Generic;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Data;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    /// <summary>
    /// Represents the options for repeating a task.
    /// This is implemented as a serializable class to allow for easy configuration in the Unity Editor.
    /// </summary>
    [Serializable]
    public class RepeatOption
    {
        /// <summary>
        /// Indicates whether to use a table for repeat configuration.
        /// </summary>
        public bool useTable;

        /// <summary>
        /// The table containing repeat configurations.
        /// </summary>
        public Table table;

        /// <summary>
        /// The number of times to repeat the task.
        /// </summary>
        public int numberOfRepeat = 3;
    }

    /// <summary>
    /// Represents a task that can be repeated multiple times.
    /// </summary>
    public class RepeatTask : BaseTask
    {
        /// <summary>
        /// Settings for the repeat task.
        /// </summary>
        [Header("Repeat Task Settings")]
        [SerializeField] private RepeatOption repeatOption;

        /// <summary>
        /// The name suffix of the output csv.
        ///
        /// <example>
        /// If set to "defaultSet", the output will be saved as "[timestamps]_id_defaultSet.csv"
        /// </example>
        /// </summary>
        [SerializeField] private string outputSetName = "defaultSet";

        /// <summary>
        /// The list of columns for the output csv columns.
        /// </summary>
        [SerializeField] private List<string> outputColumns;

        /// <summary>
        /// The current subtask number (0-indexed).
        /// </summary>
        [Tooltip("Current SubTask Number (0-indexed)")]
        [NotEditable] public int currentRepeat = 1;

        /// <summary>
        /// The current subtask number (1-indexed).
        /// </summary>
        [Tooltip("Current SubTask Number (1-indexed)")]
        [NotEditable] public int currentSubTaskNumber = 1;

        /// <summary>
        /// Indicates whether to show debug information.
        /// </summary>
        [SerializeField] private bool showDebug = true;

        /// <summary>
        /// Dictionary to store context information.
        /// This Context will be cleared after each repeat.
        /// </summary>
        public readonly Dictionary<string, string> Context = new();

        /// <summary>
        /// Gets the current table being used for repeat configuration.
        /// </summary>
        public Table CurrentTable => repeatOption.table;

        /// <summary>
        /// Gets the current data from the table.
        /// </summary>
        public DataFrame CurrentData => repeatOption.table.Enumerator.GetCurrent();

        /// <summary>
        /// Gets the current data from the table by index.
        /// </summary>
        /// <param name="tableIndex">Index of the table.</param>
        /// <returns>Data from the specified table.</returns>
        public DataFrame CurrentDataByTable(int tableIndex) => repeatOption.table.Enumerator.GetCurrentByTable(tableIndex);

        /// <summary>
        /// Delegate for executing all tasks.
        /// </summary>
        private delegate IEnumerator ExecuteAllDelegate();

        /// <summary>
        /// Instance of the execute all delegate.
        /// </summary>
        private ExecuteAllDelegate _executeAll;

        /// <summary>
        /// Prepares the task for execution.
        /// </summary>
        protected override void Prepare()
        {
            SetTaskType(TaskType.Structural);
            base.Prepare();
            if (repeatOption.useTable && repeatOption.table)
            {
                repeatOption.numberOfRepeat = repeatOption.table.Count;
                _executeAll = ExecuteByTable;
            }
            else
            {
                _executeAll = ExecuteByRepeat;
            }

            if (string.IsNullOrEmpty(outputSetName))
            {
                Logger.W("output", "Output Set Name is not set. Using default name.");
                outputSetName = "defaultSet";
            }

            if (outputColumns == null || outputColumns.Count == 0)
            {
                Logger.W("output", "Output Columns are not set. Using default column names.");
                outputColumns = new List<string> { "Repeat", "SubTask" };
            }

            Logger.BeginDataSet(outputSetName, outputColumns);
        }

        /// <summary>
        /// Executes the task by iterating through the table.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
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
                LogContext();
                ResetSubtasks();
                currentRepeat++;
            }
            isSubTaskRunning = false;
        }

        /// <summary>
        /// Executes the task by repeating a specified number of times.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator ExecuteByRepeat()
        {
            isSubTaskRunning = true;
            while (currentRepeat <= repeatOption.numberOfRepeat)
            {
                yield return ExecuteSubTasks();
                LogContext();
                ResetSubtasks();
                currentRepeat++;
            }
            isSubTaskRunning = false;
        }

        /// <summary>
        /// Executes all tasks, including subtasks.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        public override IEnumerator ExecuteAll()
        {
            if (!_enable) yield break;

            Prepare();

            isSubTaskRunning = true;
            yield return _executeAll?.Invoke();
            isSubTaskRunning = false;

            Finish();
        }

        /// <summary>
        /// Executes all subtasks.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator ExecuteSubTasks()
        {
            foreach (var subTask in _subTasks)
            {
                yield return subTask.ExecuteAll();
                currentSubTaskNumber++;
            }
        }

        /// <summary>
        /// Logs the context data for the current execution.
        /// </summary>
        private void LogContext()
        {
            foreach (var column in outputColumns)
            {
                var value = Context.GetValueOrDefault(column, "N/A").Trim('"');
                value = $"\"{value}\"";
                Logger.SetData(outputSetName, column, value);
            }

            Logger.LogDataRow(outputSetName);
        }

        /// <summary>
        /// Resets the state of all subtasks.
        /// </summary>
        private void ResetSubtasks()
        {
            currentSubTaskNumber = 1;
            foreach (var task in _subTasks)
            {
                task.Reset();
            }
            Context.Clear();
        }

        /// <summary>
        /// Finishes the task, performing any necessary cleanup.
        /// </summary>
        public override void Finish()
        {
            base.Finish();
            Logger.EndDataSet(outputSetName);
        }

        /// <summary>
        /// Unity method for rendering GUI elements.
        /// </summary>
        private void OnGUI()
        {
            if (!isSubTaskRunning || !showDebug) return;
            GUI.Label(new Rect(10, 10, 100, 20), $"Repeat: {currentRepeat}/{repeatOption.numberOfRepeat}");
            GUI.Label(new Rect(10, 30, 100, 20), $"SubTask: {currentSubTaskNumber}/{_subTasks.Count}");
        }
    }
}
