using UnityEngine;
using UnityEngine.Assertions;

namespace Taylors.Scripts
{
    public class CloseControllerTrigger : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private TargetObjectSelection targetObjectSelection;
        private Collider _collider;
        private void Start()
        {
            Assert.IsNotNull(targetObjectSelection, "TargetObjectSelection is not set");
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, "Collider is not set");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ControllerCollider"))
            {
                targetObjectSelection.HighlightButton(_collider);
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
