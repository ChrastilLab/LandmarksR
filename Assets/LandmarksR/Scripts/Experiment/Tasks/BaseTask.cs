using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Log;
using LandmarksR.Scripts.Player;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class BaseTask : MonoBehaviour
    {

        [SerializeField] protected bool enable = true;
        [NotEditable] public uint id;

        protected Settings settings;
        protected Experiment experiment;
        protected PlayerController playerController;
        protected PlayerEventController playerEvent;
        protected Hud hud;
        protected ExperimentLogger logger;

        // Subtasks (if any) are directly loaded from the children of the task
        protected List<BaseTask> subTasks;

        [Header("Time")]
        [SerializeField] protected float timer = Mathf.Infinity;
        [NotEditable, SerializeField] protected float elapsedTime;

        protected virtual void Start()
        {
            subTasks = transform.Cast<Transform>()
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
            settings = Settings.Instance;
            experiment = Experiment.Instance;
            playerController = experiment.playerController;
            playerEvent = playerController.playerEvent;
            hud = playerController.hud;
            logger = ExperimentLogger.Instance;

            logger.I("task", $"{name} started");

            isCompleted = false;
            isRunning = true;
            StartTimer();
        }

        protected virtual void Finish()
        {
            logger.I("task", $"{name} Finished");
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
            foreach (var task in subTasks)
            {
                yield return task.ExecuteAll();
            }
            isSubTaskRunning = false;

            Finish();
        }

        //method to join all the subtasks name
        private string GetSubTasksName()
        {
            return subTasks.Aggregate("", (current, task) => current + (task.name + " "));
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
