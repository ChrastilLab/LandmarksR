using System;
using LandmarksR.Scripts.Experiment.Tasks;
using LandmarksR.Scripts.Player;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{

    public class Experiment : MonoBehaviour
    {
        public static Experiment Instance => _instance ??= BuildExperiment();
        private static Experiment _instance;

        // Experiment-related variables


        [SerializeField] public PlayerController playerController;

        // Singleton-related methods
        private static Experiment BuildExperiment()
        {
            return new GameObject("Experiment").AddComponent<Experiment>();
        }

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
                throw new Exception("No Collection Component found");
            }

            StartCoroutine(rootTask.ExecuteAll());
        }
    }
}
