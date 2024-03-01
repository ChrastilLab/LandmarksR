using System;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{
    public class Experiment : MonoBehaviour
    {
        public static Experiment Instance => _instance != null ? _instance : BuildExperiment();
        private static Experiment _instance;

        // Experiment-related variables


        // Player variables
        [SerializeField] public DisplayMode displayMode;
        [SerializeField] public PlayerController playerController;
        [SerializeField] private Task rootTask;

        // Singleton-related methods
        private static Experiment BuildExperiment()
        {
            return new GameObject("Experiment").AddComponent<Experiment>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                _instance = this;
            }
        }

        private void Start()
        {
            playerController.SwitchDisplayMode(displayMode);
            StartCoroutine(rootTask.ExecuteAll());
        }

        private void Update()
        {
        }
    }
}
