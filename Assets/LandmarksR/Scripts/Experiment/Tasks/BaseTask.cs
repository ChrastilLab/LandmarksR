using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Experiment.Log;
using LandmarksR.Scripts.Player;
using UnityEngine.Assertions;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class BaseTask : MonoBehaviour
    {

        protected bool _enable;
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

        private void Awake()
        {
            _enable = true;
        }

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
        private bool isPrepared;

        protected virtual void Prepare()
        {
            Settings = Settings.Instance;
            Experiment = Experiment.Instance;
            Player = Experiment.playerController;
            PlayerEvent = Player.playerEvent;
            HUD = Player.hud;
            Logger = ExperimentLogger.Instance;

            Assert.IsNotNull(Settings, $"{name} is missing a reference to Settings");
            Assert.IsNotNull(Experiment, $"{name} is missing a reference to Experiment");
            Assert.IsNotNull(Player, $"{name} is missing a reference to Player");
            Assert.IsNotNull(PlayerEvent, $"{name} is missing a reference to PlayerEvent");
            Assert.IsNotNull(HUD, $"{name} is missing a reference to HUD");
            Assert.IsNotNull(Logger, $"{name} is missing a reference to Logger");
            Logger.I("task", $"({name}) Started");


            isCompleted = false;
            isRunning = true;
            isPrepared = true;
            elapsedTime = 0;

            if (randomizeTimer)
            {
                timer = UnityEngine.Random.Range(minTimer, maxTimer);
            }

            StartTimer();
        }

        protected virtual void Finish()
        {
            if (!isPrepared)
            {
                UnityEngine.Debug.LogWarning("Task not prepared before finishing. This may cause issues.");
                return;
            }
            Logger.I("task", $"({name}) Finished");
            isCompleted = true;
            isPrepared = false;
        }

        public virtual void Reset()
        {
            isCompleted = false;
        }

        public virtual IEnumerator ExecuteAll()
        {
            if (!_enable) yield break;

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
