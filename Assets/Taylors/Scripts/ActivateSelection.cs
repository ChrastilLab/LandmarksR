using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Taylors.Scripts
{
    public class ActivateSelection : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private bool hideTargetOnStart = true;
        private GameObject selectionMenu;
        private TargetObjectSelection targetObjectSelection;
        private void Start()
        {
            Assert.IsTrue(target != null, "Target is not set");
            if (hideTargetOnStart) target.SetActive(false);

            Assert.IsTrue(transform.childCount > 0, "Selection Menu is not set");

            selectionMenu = transform.GetChild(0).gameObject;
            selectionMenu.SetActive(false);


            targetObjectSelection = selectionMenu.GetComponent<TargetObjectSelection>();
            Assert.IsNotNull(targetObjectSelection, "TargetObjectSelection is not set");

            targetObjectSelection.targetSelected.AddListener((string targetName) =>
            {
                if (target.name.IndexOf(targetName, StringComparison.Ordinal) >= 0)
                {
                    target.SetActive(true);
                }
            });
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
