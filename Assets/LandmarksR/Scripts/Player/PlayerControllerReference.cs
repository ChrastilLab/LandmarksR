using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    public class PlayerControllerReference: MonoBehaviour
    {
        [SerializeField]
        public Camera mainCamera;

        [SerializeField]
        public Canvas canvas;

        [SerializeField]
        public Collider playerCollider;

        [SerializeField]
        public GameObject leftHandAnchor;

        [SerializeField]
        public GameObject rightHandAnchor;


    }
}
