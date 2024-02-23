using UnityEngine;

#if UNITY_EDITOR
using LandmarksR.Scripts.Inspector;
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

        public uint ID
        {
            get => id;
            set => id = value;
        }

        public void Prepare()
        {
            IsCompleted = false;
        }

        public void Run()
        {
        }

        public bool IsCompleted { get; private set; }

        public void Skip()
        {
        }

        public void CleanUp()
        {
        }
    }
}
