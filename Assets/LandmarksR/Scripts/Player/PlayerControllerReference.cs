using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    /// <summary>
    /// Holds references to key components of the player controller.
    /// </summary>
    public class PlayerControllerReference : MonoBehaviour
    {
        /// <summary>
        /// The main camera associated with the player.
        /// </summary>
        [SerializeField]
        public Camera mainCamera;

        /// <summary>
        /// The canvas associated with the player.
        /// </summary>
        [SerializeField]
        public Canvas canvas;

        /// <summary>
        /// The collider associated with the player.
        /// </summary>
        [SerializeField]
        public Collider playerCollider;

        /// <summary>
        /// The left hand anchor for VR interactions.
        /// </summary>
        [SerializeField]
        public GameObject leftHandAnchor;

        /// <summary>
        /// The right hand anchor for VR interactions.
        /// </summary>
        [SerializeField]
        public GameObject rightHandAnchor;
    }
}
