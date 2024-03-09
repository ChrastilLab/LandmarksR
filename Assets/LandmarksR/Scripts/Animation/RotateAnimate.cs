using UnityEngine;

namespace LandmarksR.Scripts.Animation
{
    public class RotateAnimate : MonoBehaviour
    {
        public float speed = 2f;
        public Vector3 axis = Vector3.forward;

        private void Update()
        {
            // Rotate the object around its local axis
            transform.Rotate(axis, speed * Time.deltaTime);
        }
    }
}
