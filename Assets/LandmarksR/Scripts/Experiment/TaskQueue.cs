using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{
    public class TaskQueue
    {
        private List<Task> _tasks;
        private Task _currentTaskCoroutine;


        public TaskQueue(List<Task> tasks)
        {
            _tasks = tasks;
        }

        public IEnumerator ExecuteTasks()
        {
            foreach (var task in _tasks)
            {
                _currentTaskCoroutine = task;
                task.Prepare();
                task.Run();
                yield return new WaitUntil(() => task.IsCompleted);
                task.CleanUp();
            }
        }
    }
}
