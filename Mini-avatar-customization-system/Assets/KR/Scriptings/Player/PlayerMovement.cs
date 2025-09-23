namespace KR.Scriptings.Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerAnimation))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Player Movement Settings")]
        [SerializeField] private InputAction inputMovement;
        [SerializeField] private InputAction inputSprint;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float sprintSpeed = 9f;
        [SerializeField] private float rotationSmoothTime = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private PlayerAnimation playerAnimation;

        public static readonly float TOLERANCE = 0.01f;

        private Vector2 currentInputMovement;
        private Vector3 currentVelocity;
        private float currentSpeed;
        private Transform mainCameraTransform;

        private void OnEnable()
        {
            inputMovement.Enable();
            inputMovement.performed += MovementPerformed;
            inputMovement.canceled += MovementPerformed;

            inputSprint.Enable();
            inputSprint.performed += SprintPerformed;
            inputSprint.canceled += SprintPerformed;
        }

        private void OnDisable()
        {
            inputMovement.Disable();
            inputMovement.performed -= MovementPerformed;
            inputMovement.canceled -= MovementPerformed;

            inputSprint.Disable();
            inputSprint.performed -= SprintPerformed;
            inputSprint.canceled -= SprintPerformed;
        }

        private void OnDestroy()
        {
            inputMovement.Dispose();
            inputSprint.Dispose();
        }

        private void MovementPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                currentInputMovement = context.ReadValue<Vector2>(); // Get movement input
            }
            else if (context.canceled)
            {
                currentInputMovement = Vector2.zero; // Reset movement input on cancel
            }
        }

        private void SprintPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                currentSpeed = sprintSpeed; // Set to sprint speed
            }
            else if (context.canceled)
            {
                currentSpeed = speed; // Reset to normal speed
            }
        }

        private void Start()
        {
            mainCameraTransform = Camera.main.transform; // Cache main camera transform
            currentSpeed = speed; // Initialize current speed
        }

        private void FixedUpdate()
        {
            ApplyGravityPhysics(); // Apply gravity in FixedUpdate for consistent physics
            Movement(); // Handle movement
        }

        private void Update()
        {
            playerAnimation.Movement(currentInputMovement, controller.velocity); // Update animation based on movement
        }

        private void Movement()
        {
            // Early out if no input
            if (currentInputMovement == Vector2.zero)
            {
                return;
            }

            // Determine direction to move
            Vector3 direction = new Vector3(currentInputMovement.x, 0, currentInputMovement.y).normalized;

            // Move in that direction
            if (direction.magnitude >= TOLERANCE) // Deadzone check
            {
                Rotation(direction, out float targetAngle); // Handle rotation
                Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward; // Forward direction based on rotation
                controller.Move(currentSpeed * Time.deltaTime * moveDir.normalized); // Move the player
            }
        }

        private void Rotation(Vector3 direction, out float targetAngle)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCameraTransform.eulerAngles.y; // Calculate target angle based on input and camera
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * rotationSmoothTime); // Smoothly rotate towards target angle
        }

        private void ApplyGravityPhysics()
        {
            // Apply gravity
            if (controller.isGrounded)
            {
                currentVelocity.y = 0; // Reset vertical velocity when grounded
            }
            else
            {
                currentVelocity.y += gravity * Time.deltaTime; // Apply gravity over time
            }

            controller.Move(currentVelocity * Time.deltaTime); // Apply gravity movement
        }
    }
}