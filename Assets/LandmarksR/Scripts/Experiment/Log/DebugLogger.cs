using System.Collections.Generic;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class DebugLogger: TextLogger
    {
        public static DebugLogger Instance { get; private set; }
        [SerializeField] private List<string> tagToPrint = new() {"default"};
        private bool CheckTag(string messageTag) => tagToPrint.IndexOf(messageTag) >= 0;

        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                Instance = this;
            }

            base.Awake();
            DontDestroyOnLoad(this);
        }

        public override void I(string messageTag, string message)
        {
            if (!CheckTag(messageTag)) return;
            Debug.Log($"<color=green>INFO</color> | {messageTag} | {message}");
            base.I(messageTag, message);
        }

        public override void W(string messageTag, string message)
        {
            if (!CheckTag(messageTag)) return;
            Debug.Log($"<color=yellow>WARNING</color> | {messageTag} | {message}");
            base.W(messageTag, message);
        }

        public override void E(string messageTag, string message)
        {
            if (!CheckTag(messageTag)) return;
            Debug.Log($"<color=red>ERROR</color> | {messageTag} | {message}");
            base.E(messageTag, message);
        }

    }
}
