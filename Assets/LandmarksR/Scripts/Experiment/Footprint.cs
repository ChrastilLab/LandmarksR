using System;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment
{
    public class Footprint : MonoBehaviour
    {
        // Start is called before the first frame update
        public Action<Collider> TriggerEnterAction;
        public Action<Collider> TriggerExitAction;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter "+ other.name);
            TriggerEnterAction?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("OnTriggerExit "+ other.name);
            TriggerExitAction?.Invoke(other);
        }
    }
}
