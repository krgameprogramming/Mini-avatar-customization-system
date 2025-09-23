namespace KR.Scriptings.Misc
{
    using UnityEngine;

    public class MouseVisibility : MonoBehaviour
    {
        [SerializeField] private bool disableOnStart = true;

        private void Start()
        {
            if (disableOnStart)
            {
                Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
                Cursor.visible = false;
            }
        }
    }
}