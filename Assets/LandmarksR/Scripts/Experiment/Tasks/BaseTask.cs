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

        protected Settings Settings { get; private set; }
        private Experiment Experiment { get; set; }
        protected PlayerController Player { get; private set; }
        protected PlayerEventController PlayerEvent { get; private set; }
        protected Hud HUD { get; private set; }
        protected ExperimentLogger Logger { get; set; }

        // Subtasks (if any) are directly loaded from the children of the task
        protected List<BaseTask> _subTasks = new();

        [Header("Time")]
        [SerializeField] protected float timer = Mathf.Infinity;
        [SerializeField] protected bool randomizeTimer;
        [SerializeField] private float minTimer = 0;
        [SerializeField] private float maxTimer = 10;
        [NotEditable, SerializeField] protected float elapsedTime;

        protected virtual void Start()
        {
            _subTasks = transform.Cast<Transform>()
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
            Settings = Settings.Instance;
            Experiment = Experiment.Instance;
            Player = Experiment.playerController;
            PlayerEvent = Player.playerEvent;
            HUD = Player.hud;
            Logger = ExperimentLogger.Instance;

            Logger.I("task", $"({name}) Started");

            isCompleted = false;
            isRunning = true;
            elapsedTime = 0;

            if (randomizeTimer)
            {
                timer = UnityEngine.Random.Range(minTimer, maxTimer);
            }

            StartTimer();
        }

        protected virtual void Finish()
        {
            Logger.I("task", $"({name}) Finished");
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
            if (_subTasks == null)
            {
                Finish();
                yield break;
            }

            foreach (var task in _subTasks)
            {
                yield return task.ExecuteAll();
            }
            isSubTaskRunning = false;

            Finish();
        }

        //method to join all the subtasks name
        private string GetSubTasksName()
        {
            return _subTasks.Aggregate("", (current, task) => current + (task.name + " "));
        }

        protected void StartTimer()
        {
            StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {
            while (elapsedTime < timer && isRunning)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            isRunning = false;
        }
    }
}
