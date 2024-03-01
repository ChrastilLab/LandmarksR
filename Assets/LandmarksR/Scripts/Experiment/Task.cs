using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Attributes;
using LandmarksR.Scripts.Utility;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace LandmarksR.Scripts.Experiment
{
    public class Task : MonoBehaviour
    {
        [
#if UNITY_EDITOR
            NotEditable,
#endif
            SerializeField
        ]
        private uint id;

        private List<Task> _subTasks;

        private void Start()
        {
            _subTasks = transform.Cast<Transform>()
                .OrderBy(tr => tr.GetSiblingIndex())
                .Select(tr=> tr.GetComponent<Task>())
                .ToList();
        }

        public uint ID
        {
            get => id;
            set => id = value;
        }


        [
#if UNITY_EDITOR
            NotEditable,
#endif
            SerializeField
        ]
        protected bool IsCompleted;

        protected virtual void Prepare()
        {
            DebugLogger.Instance.Log($"{name} Preparing", "task");
        }

        protected virtual void CleanUp()
        {
            
            DebugLogger.Instance.Log($"{name} Cleaning", "task");
        }

        private void Update()
        {
        }


        public IEnumerator ExecuteAll()
        {
            Prepare();
            
            // Wait for the update function to update completion status
            yield return new WaitUntil(() => IsCompleted); 
            
            foreach (var task in _subTasks)
            {
                yield return task.ExecuteAll();
            }
            
            CleanUp();
        }

    }
}
