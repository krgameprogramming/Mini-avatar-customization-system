namespace KR.Scriptings.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using KR.Scriptings.Avatar;

    public class AvatarCustomizationUI : MonoBehaviour
    {
        [Header("Avatar Customization UI Settings")]
        [SerializeField] private AvatarCustomization avatarCustomization;

        [SerializeField] private Button[] buttonIncrements;
        [SerializeField] private Button[] buttonDecrements;
        [SerializeField] private TextMeshProUGUI[] texts;

        private readonly string defaultText = "Default";

        private void Start()
        {
            AddButtonListener(); // Add button listeners to each button
            foreach (TextMeshProUGUI text in texts)
            {
                text.text = defaultText; // Set all texts to default
            }
        }

        private void AddButtonListener()
        {
            // Add Increment Function to each incerement buttons
            for (int i = 0; i < buttonIncrements.Length; i++)
            {
                // Use a temporary variable to capture the value of 'i'
                int buttonIndex = i;

                buttonIncrements[i].onClick.AddListener(() =>
                {
                    Increment((KR.Scriptings.Avatar.Avatar)buttonIndex); // true for increment
                });
            }

            // Add Increment Function to each incerement buttons
            for (int i = 0; i < buttonDecrements.Length; i++)
            {
                // Use a temporary variable to capture the value of 'i'
                int buttonIndex = i;

                buttonDecrements[i].onClick.AddListener(() =>
                {
                    Increment((KR.Scriptings.Avatar.Avatar)buttonIndex, false); // false for decrement
                });
            }
        }

        private void Increment(KR.Scriptings.Avatar.Avatar avatar, bool isIncrement = true) // Increment or Decrement the avatar customization
        {
            avatarCustomization.SetAvatarCustom(avatar, isIncrement ? 1 : -1, out AvatarCustom avatarCustom); // set avatar custom and get the current avatar custom
            texts[(int)avatar].text = avatarCustom.Equals(default(AvatarCustom)) ? defaultText : avatarCustom.renderer.name; // set the text to the current avatar custom name or default text
            CheckMutualExclusivity(avatar); // check for mutual exclusivity between Outfit and Top/Bottom/Shoes
        }

        private void CheckMutualExclusivity(KR.Scriptings.Avatar.Avatar avatar) // Disable/Enable buttons based on mutual exclusivity rules
        {
            switch (avatar)
            {
                case Scriptings.Avatar.Avatar.Outfit:
                    buttonIncrements[(int)Scriptings.Avatar.Avatar.Top].interactable = buttonDecrements[(int)Scriptings.Avatar.Avatar.Top].interactable = texts[(int)avatar].text == defaultText;
                    buttonIncrements[(int)Scriptings.Avatar.Avatar.Bottom].interactable = buttonDecrements[(int)Scriptings.Avatar.Avatar.Bottom].interactable = texts[(int)avatar].text == defaultText;
                    buttonIncrements[(int)Scriptings.Avatar.Avatar.Shoes].interactable = buttonDecrements[(int)Scriptings.Avatar.Avatar.Shoes].interactable = texts[(int)avatar].text == defaultText;
                    break;
                case Scriptings.Avatar.Avatar.Top:
                case Scriptings.Avatar.Avatar.Bottom:
                case Scriptings.Avatar.Avatar.Shoes:
                    buttonIncrements[(int)Scriptings.Avatar.Avatar.Outfit].interactable = buttonDecrements[(int)Scriptings.Avatar.Avatar.Outfit].interactable =
                    texts[(int)Scriptings.Avatar.Avatar.Top].text == defaultText &&
                    texts[(int)Scriptings.Avatar.Avatar.Bottom].text == defaultText &&
                    texts[(int)Scriptings.Avatar.Avatar.Shoes].text == defaultText;
                    break;
            }
        }
    }
}