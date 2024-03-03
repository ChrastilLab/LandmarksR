using LandmarksR.Scripts.Utility;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Tasks
{
    public class DummyTask: BaseTask
    {
        private void Update()
        {
            isRunning = false;
        }
    }
}
