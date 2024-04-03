using UnityEngine;

namespace Taylors.Scripts
{
    public class ActivateSelection : MonoBehaviour
    {
        private GameObject selectionMenu;
        private void Start()
        {
            Debug.Assert(transform.childCount > 0, "Selection Menu is not set");
            selectionMenu = transform.GetChild(0).gameObject;
            selectionMenu.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerCollider"))
            {
                selectionMenu.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlayerCollider"))
            {
                selectionMenu.SetActive(false);
            }
        }
    }
}
