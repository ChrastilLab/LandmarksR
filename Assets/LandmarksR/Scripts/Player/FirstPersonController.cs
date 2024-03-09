using System;
using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    public class FirstPersonController : MonoBehaviour
    {
        private CharacterController _characterController;
        [SerializeField] private Camera cam;
        [SerializeField] private bool lockVerticalRotation;
        [SerializeField] private bool enableGravity = true;

        public float mouseSensitivity = 5;
        public float moveSpeed = 10f;
        private float _xRotation;

        private Vector3 _gravityVelocity;

        public bool enableControl;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }


        private void Update()
        {
            if (!enableControl) return;
            HandleMovement();
            HandleGravity();
            HandleRotation();
            HandleCursor();
        }

        public void Teleport(Vector3 position, Vector3 rotation)
        {
            _characterController.enabled = false;
            transform.rotation = Quaternion.Euler(rotation);
            transform.position = position;
            _characterController.enabled = true;
        }

        private void HandleMovement()
        {
            var move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
            _characterController.Move(move * (Time.deltaTime * moveSpeed));
        }

        private void HandleRotation()
        {
            mouseSensitivity = Mathf.Clamp(mouseSensitivity, 1f, 20f);
            var mouseX = 100f * Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            var mouseY = 0f;
            if (!lockVerticalRotation)
            {
                mouseY = 100f * Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            }

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f); // Clamp the vertical axis

            cam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        private static void HandleCursor()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void HandleGravity()
        {
            if (!enableGravity) return;
            if (_characterController.isGrounded && _gravityVelocity.y < 0)
            {
                _gravityVelocity.y = -2f;
            }
            else
            {
                _gravityVelocity.y += Physics.gravity.y * Time.deltaTime;
            }

            _characterController.Move(_gravityVelocity * Time.deltaTime);
        }
    }
}
