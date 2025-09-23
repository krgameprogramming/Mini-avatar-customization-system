namespace KR.Scriptings.Misc
{
    using UnityEngine;

    public class MouseVisibility : MonoBehaviour
    {
        [SerializeField] private bool disableOnStart = true;

        private void Start()
        {
            Enable(!disableOnStart);
        }

        public void Enable(bool isEnabled)
        {
            switch (isEnabled)
            {
                case true:
                    Cursor.lockState = CursorLockMode.None; // Unlock
                    break;
                case false:
                    Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
                    break;
            }
            Cursor.visible = isEnabled;
        }
    }
}