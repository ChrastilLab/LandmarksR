using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    /// <summary>
    /// Manages the first-person controller for player movement and interaction.
    /// </summary>
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

        /// <summary>
        /// Unity Start method. Initializes the character controller and locks the cursor.
        /// </summary>
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Unity Update method. Handles player input for movement, gravity, rotation, and cursor control.
        /// </summary>
        private void Update()
        {
            if (!enableControl) return;
            HandleMovement();
            HandleGravity();
            HandleRotation();
            HandleCursor();
        }

        /// <summary>
        /// Teleports the player to a specified position and rotation.
        /// </summary>
        /// <param name="position">The target position.</param>
        /// <param name="rotation">The target rotation.</param>
        public void Teleport(Vector3 position, Vector3 rotation)
        {
            _characterController.enabled = false;
            transform.rotation = Quaternion.Euler(rotation);
            transform.position = position;
            _characterController.enabled = true;
        }

        /// <summary>
        /// Handles player movement based on input.
        /// </summary>
        private void HandleMovement()
        {
            var move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
            _characterController.Move(move * (Time.deltaTime * moveSpeed));
        }

        /// <summary>
        /// Handles player rotation based on mouse input.
        /// </summary>
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

        /// <summary>
        /// Handles cursor locking and unlocking based on input.
        /// </summary>
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

        /// <summary>
        /// Handles gravity effects on the player.
        /// </summary>
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
