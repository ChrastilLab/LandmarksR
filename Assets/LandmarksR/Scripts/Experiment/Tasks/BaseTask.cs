using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Log;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class BaseTask : MonoBehaviour
    {
        [SerializeField] protected bool enable = true;
        [NotEditable, SerializeField] private uint id;

        public uint ID
        {
            get => id;
            set => id = value;
        }

        protected List<BaseTask> SubTasks;
        [SerializeField] protected float timer = Mathf.Infinity;
        [NotEditable, SerializeField] protected float elapsedTime;

        protected virtual void Start()
        {
            SubTasks = transform.Cast<Transform>()
                .OrderBy(tr => tr.GetSiblingIndex())
                .Select(tr => tr.GetComponent<BaseTask>())
                .Where(component => component != null)
                .ToList();
        }

        [NotEditable, SerializeField] protected bool isRunning;
        [NotEditable, SerializeField] protected bool isSubTaskRunning;
        [NotEditable, SerializeField] protected bool isCompleted;

        protected virtual void Prepare()
        {
            DebugLogger.Instance.I("task", $"{name} Started");
            isCompleted = false;
            isRunning = true;
            StartTimer();
        }

        protected virtual void Finish()
        {
            DebugLogger.Instance.I("task", $"{name} Finished");
            isCompleted = true;
        }

        public virtual void Reset()
        {
            isCompleted = false;
        }

        public virtual IEnumerator ExecuteAll()
        {
            if (!enable) yield break;

            Prepare();

            // Wait for the update function to update completion status
            yield return new WaitUntil(() => !isRunning);

            isSubTaskRunning = true;
            foreach (var task in SubTasks)
            {
                yield return task.ExecuteAll();
            }
            isSubTaskRunning = false;

            Finish();
        }

        //method to join all the subtasks name
        private string GetSubTasksName()
        {
            return SubTasks.Aggregate("", (current, task) => current + (task.name + " "));
        }

        private void StartTimer()
        {
            StartCoroutine(TimerCoroutine());
        }
        private IEnumerator TimerCoroutine()
        {
            while (elapsedTime < timer)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            isRunning = false;
        }
    }
}
