using System;
using LandmarksR.Scripts.Experiment.Tasks;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{
    /// <summary>
    /// Manages the experiment lifecycle, ensuring there is only one instance running.
    /// </summary>
    public class Experiment : MonoBehaviour
    {
        /// <summary>
        /// The singleton instance of the Experiment class.
        /// </summary>
        public static Experiment Instance => _instance ??= BuildExperiment();
        private static Experiment _instance;

        /// <summary>
        /// The player controller associated with the experiment.
        /// </summary>
        [SerializeField] public PlayerController playerController;

        /// <summary>
        /// Builds the singleton instance of the Experiment class.
        /// </summary>
        /// <returns>A new instance of the Experiment class.</returns>
        private static Experiment BuildExperiment()
        {
            return new GameObject("Experiment").AddComponent<Experiment>();
        }

        /// <summary>
        /// Unity Awake method. Ensures there is only one instance of the Experiment class.
        /// </summary>
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        /// <summary>
        /// Unity Start method. Initializes the experiment and starts the root task.
        /// </summary>
        private void Start()
        {
            Debug.Assert(playerController != null, "PlayerController is not set");

            var rootTaskGameObject = GameObject.FindGameObjectWithTag("RootTask");
            if (rootTaskGameObject == null)
            {
                throw new Exception("No RootTask GameObject found");
            }

            var rootTask = rootTaskGameObject.GetComponent<RootTask>();
            if (rootTask == null)
            {
                throw new Exception("No RootTask component found");
            }

            StartCoroutine(rootTask.ExecuteAll());
        }
    }
}
