using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    public class SubController: MonoBehaviour
    {
        [SerializeField]
        public Camera mainCamera;

        [SerializeField]
        public Canvas canvas;

        [SerializeField]
        public Collider playerCollider;

    }
}
