using System;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{
    public class Footprint : MonoBehaviour
    {
        // Start is called before the first frame update
        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter "+ other.name);
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("OnTriggerExit "+ other.name);
            onTriggerExit?.Invoke(other);
        }
    }
}
