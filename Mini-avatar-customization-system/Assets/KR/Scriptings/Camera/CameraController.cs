namespace KR.Scriptings.Camera
{
    using KR.Scriptings.Misc;
    using Unity.Cinemachine;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class CameraController : MonoBehaviour
    {
        [Header("Camera Controller Settings")]
        [SerializeField] private InputAction inputEnableAxis;
        [SerializeField] private CinemachineInputAxisController cinemachineInputAxisController;
        [SerializeField] private MouseVisibility mouseVisibility;

        private void OnEnable()
        {
            inputEnableAxis.Enable();
            inputEnableAxis.performed += EnableAxisPerformed;
            inputEnableAxis.canceled += EnableAxisPerformed;
        }

        private void OnDisable()
        {
            inputEnableAxis.Disable();
            inputEnableAxis.performed -= EnableAxisPerformed;
            inputEnableAxis.canceled -= EnableAxisPerformed;
        }

        private void OnDestroy()
        {
            inputEnableAxis.Dispose();
        }

        private void Start()
        {
            cinemachineInputAxisController.enabled = false;
        }

        private void EnableAxisPerformed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                cinemachineInputAxisController.enabled = true;
                mouseVisibility.Enable(false);
            }
            else if (context.canceled)
            {
                cinemachineInputAxisController.enabled = false;
                mouseVisibility.Enable(true);
            }
        }
    }
}