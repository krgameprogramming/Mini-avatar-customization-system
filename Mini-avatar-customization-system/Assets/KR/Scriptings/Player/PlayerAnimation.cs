namespace KR.Scriptings.Player
{
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        [Header("Player Animation Settings")]
        [SerializeField] private Animator animator;
        [SerializeField] private float animationSpeedSmoothTime = 5f; // Smoother when starting
        [SerializeField] private float animationSpeedSmoothTimeRelease = 10f; // Smoother when stopping
        [SerializeField] private float animationMaxSpeed = 5f; // Maximum speed for animation purposes

        private readonly int speedHash = Animator.StringToHash("Speed"); // Hash for the "Speed" parameter in the animator
        private float TOLERANCE => PlayerMovement.TOLERANCE;

        public void Movement(Vector2 inputMovement, Vector3 velocity)
        {
            if (inputMovement == Vector2.zero && animator.GetFloat(speedHash) == 0)
            {
                return; // Early out if no input and speed is already zero
            }

            float currentSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude; // Current horizontal speed using velocity
            float speedSmoothTime = inputMovement == Vector2.zero ? animationSpeedSmoothTimeRelease : animationSpeedSmoothTime; // Choose smooth time based on input
            float lerpedSpeed = Mathf.Lerp(animator.GetFloat(speedHash), currentSpeed, Time.deltaTime * speedSmoothTime); // Smoothly interpolate speed

            if (lerpedSpeed <= TOLERANCE)
            {
                lerpedSpeed = 0; // Snap to zero if very small
            }
            else if (lerpedSpeed >= animationMaxSpeed)
            {
                lerpedSpeed = animationMaxSpeed; // Clamp to max speed
            }
            animator.SetFloat(speedHash, lerpedSpeed); // Update animator speed parameter
        }
    }
}