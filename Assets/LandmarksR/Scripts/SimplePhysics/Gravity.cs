using UnityEngine;
using UnityEngine.Assertions;

namespace LandmarksR.Scripts.SimplePhysics
{
    public class Gravity : MonoBehaviour
    {
        public float gravity = -9.81f;
        private bool _isGrounded = false;

        private Vector3 _velocity;
        private Collider _collider;

        [SerializeField] private LayerMask groundLayer;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"{name} is missing a collider in Gravity script");
        }

        private void Update()
        {
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            _velocity.y += gravity * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;

            if (CheckGrounded())
            {
                Debug.Log("Grounded");
                _velocity.y = Mathf.Max(_velocity.y, 0);
            }
        }

        private bool CheckGrounded()
        {
            return Physics.CheckBox(_collider.bounds.center, _collider.bounds.extents, Quaternion.identity, groundLayer);
        }


    }
}
