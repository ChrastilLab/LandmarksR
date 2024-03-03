using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Utility;
using UnityEngine;
using LandmarksR.Scripts.Attributes;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class BaseTask : MonoBehaviour
    {
        [SerializeField] private bool enable = true;
        [NotEditable, SerializeField] private uint id;

        public uint ID
        {
            get => id;
            set => id = value;
        }

        protected List<BaseTask> SubTasks;

        protected virtual void Start()
        {
            SubTasks = transform.Cast<Transform>()
                .OrderBy(tr => tr.GetSiblingIndex())
                .Select(tr => tr.GetComponent<BaseTask>())
                .Where(component => component != null)
                .ToList();
        }

        [NotEditable, SerializeField] protected bool isRunning;
        [NotEditable, SerializeField] protected bool isCompleted;

        protected virtual void Prepare()
        {
            DebugLogger.Instance.Log($"{name} Preparing", "task");
            isRunning = true;
        }

        protected virtual void CleanUp()
        {
            DebugLogger.Instance.Log($"{name} CleanUp", "task");
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
            yield return new WaitUntil(() => isRunning == false);

            DebugLogger.Instance.Log("Subtasks: " + GetSubTasksName(), "task");
            foreach (var task in SubTasks)
            {
                yield return task.ExecuteAll();
            }

            CleanUp();
        }

        //method to join all the subtasks name
        public string GetSubTasksName()
        {
            string subTasksName = "";
            foreach (var task in SubTasks)
            {
                subTasksName += task.name + " ";
            }
            return subTasksName;
        }
    }
}
