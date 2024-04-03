using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Taylors.Scripts
{
    public class CloseControllerTrigger : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private TargetObjectSelection targetObjectSelection;
        private void Start()
        {
            Debug.Assert(targetObjectSelection != null, "TargetObjectSelection is not set");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ControllerCollider"))
            {
                targetObjectSelection.HighlightButton(name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("ControllerCollider"))
            {
                targetObjectSelection.UnhighlightAllButtons();
            }
        }
    }
}
