using System;
using System.Collections.Generic;
using UnityEngine;

namespace LandmarksR.Scripts.Utility
{
    public class DebugLogger: MonoBehaviour
    {
        public static DebugLogger Instance { get; private set; }
        [SerializeField] private List<string> tagToPrint = new() {"default"};

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }
        }

        public void Log(string content, string logTag)
        {
            if (tagToPrint.IndexOf(logTag) < 0)
            {
                return;
            }

            Debug.Log($"[LandmarksR] [{logTag}] {content}");
        }

    }
}
