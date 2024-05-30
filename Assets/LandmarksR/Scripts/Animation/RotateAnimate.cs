using UnityEngine;

namespace LandmarksR.Scripts.Animation
{
    /// <summary>
    /// Rotates the GameObject around a specified axis at a specified speed.
    /// </summary>
    public class RotateAnimate : MonoBehaviour
    {
        /// <summary>
        /// The speed of the rotation.
        /// </summary>
        public float speed = 2f;

        /// <summary>
        /// The axis around which the GameObject will rotate.
        /// </summary>
        public Vector3 axis = Vector3.forward;

        /// <summary>
        /// Updates the rotation of the GameObject each frame.
        /// </summary>
        private void Update()
        {
            // Rotate the object around its local axis
            transform.Rotate(axis, speed * Time.deltaTime);
        }
    }
}
