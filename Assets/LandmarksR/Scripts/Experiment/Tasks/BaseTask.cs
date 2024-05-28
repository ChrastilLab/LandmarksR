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
    /// <summary>
    /// Base class for all experiment tasks.
    /// Handles common task functionality such as timers and subtask management.
    /// </summary>
    public class BaseTask : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether the task is enabled.
        /// </summary>
        protected bool _enable;

        /// <summary>
        /// Unique identifier for the task.
        /// </summary>
        [NotEditable]
        public uint id;

        /// <summary>
        /// Gets the settings for the task.
        /// </summary>
        protected Settings Settings { get; private set; }

        /// <summary>
        /// Gets or sets the experiment instance associated with the task.
        /// </summary>
        private Experiment Experiment { get; set; }

        /// <summary>
        /// Gets the player controller for the task.
        /// </summary>
        protected PlayerController Player { get; private set; }

        /// <summary>
        /// Gets the player event controller for the task.
        /// </summary>
        protected PlayerEventController PlayerEvent { get; private set; }

        /// <summary>
        /// Gets the HUD (Heads-Up Display) for the task.
        /// </summary>
        protected Hud HUD { get; private set; }

        /// <summary>
        /// Gets or sets the experiment logger for the task.
        /// </summary>
        protected ExperimentLogger Logger { get; set; }

        /// <summary>
        /// List of subtasks for this task.
        /// </summary>
        protected List<BaseTask> _subTasks = new();

        /// <summary>
        /// Timer settings for the task.
        /// </summary>
        [Header("Time")]
        [SerializeField]
        protected float timer = Mathf.Infinity;

        /// <summary>
        /// Indicates whether the timer should be randomized.
        /// </summary>
        [SerializeField]
        protected bool randomizeTimer;

        /// <summary>
        /// Minimum value for the timer when randomized.
        /// </summary>
        [SerializeField]
        private float minTimer = 0;

        /// <summary>
        /// Maximum value for the timer when randomized.
        /// </summary>
        [SerializeField]
        private float maxTimer = 10;

        /// <summary>
        /// Elapsed time since the task started.
        /// </summary>
        [NotEditable, SerializeField]
        protected float elapsedTime;

        /// <summary>
        /// Unity Awake method. Initializes the task.
        /// </summary>
        private void Awake()
        {
            _enable = true;
        }

        /// <summary>
        /// Unity Start method. Initializes the list of subtasks.
        /// </summary>
        protected virtual void Start()
        {
            _subTasks = transform.Cast<Transform>()
                .OrderBy(tr => tr.GetSiblingIndex())
                .Select(tr => tr.GetComponent<BaseTask>())
                .Where(component => component != null)
                .ToList();
        }

        /// <summary>
        /// Indicates whether the task is currently running.
        /// </summary>
        [NotEditable, SerializeField]
        protected bool isRunning;

        /// <summary>
        /// Indicates whether a subtask is currently running.
        /// </summary>
        [NotEditable, SerializeField]
        protected bool isSubTaskRunning;

        /// <summary>
        /// Indicates whether the task is completed.
        /// </summary>
        [NotEditable, SerializeField]
        protected bool isCompleted;

        /// <summary>
        /// Indicates whether the task has been prepared.
        /// </summary>
        private bool isPrepared;

        /// <summary>
        /// Prepares the task for execution.
        /// </summary>
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

        /// <summary>
        /// Finishes the task, performing any necessary cleanup.
        /// </summary>
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

        /// <summary>
        /// Resets the task to its initial state.
        /// </summary>
        public virtual void Reset()
        {
            isCompleted = false;
        }

        /// <summary>
        /// Executes the task and all its subtasks.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
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

        /// <summary>
        /// Gets the names of all subtasks.
        /// </summary>
        /// <returns>A string containing the names of all subtasks.</returns>
        private string GetSubTasksName()
        {
            return _subTasks.Aggregate("", (current, task) => current + (task.name + " "));
        }

        /// <summary>
        /// Starts the task timer.
        /// </summary>
        protected void StartTimer()
        {
            StartCoroutine(TimerCoroutine());
        }

        /// <summary>
        /// Coroutine to handle the task timer.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
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
