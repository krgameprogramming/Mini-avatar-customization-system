namespace KR.Scriptings.Avatar
{
    using System;
    using UnityEngine;

    [Flags]
    public enum BaseAvatar // Base avatar body parts for enabling/disabling
    {
        // Assign explicit power-of-two values
        Head = 1 << 0,          // 1
        Hands = 1 << 1,         // 2
        Arms_Lower = 1 << 2,    // 4
        Arms_Upper = 1 << 3,    // 8
        Shoulders = 1 << 4,     // 16
        Torso_Upper = 1 << 5,   // 32
        Torso_Middle = 1 << 6,  // 64
        Torso_Bottom = 1 << 7,  // 128
        Hips = 1 << 8,          // 256
        Lengs_Upper = 1 << 9,   // 512
        Legs_Knee = 1 << 10,    // 1024
        Legs_Lower = 1 << 11,   // 2048
        Legs_Feet = 1 << 12     // 4096
    }

    public enum Avatar // Avatar type for customization
    {
        Accessories,
        Head,
        Outfit,
        Top,
        Bottom,
        Shoes
    }

    [Serializable]
    public struct AvatarCustom // Struct to hold avatar customization data
    {
        public SkinnedMeshRenderer renderer; // target renderer (this case is Skin Mesh Renderer)
        public BaseAvatar baseAvatar; // to disabled when avatar custom is used
    }

    public class AvatarCustomization : MonoBehaviour
    {
        [Header("Avatar Customization Settings")]
        [Header("Base Avatar Skin Mesh Renderer")]
        [SerializeField] private SkinnedMeshRenderer[] baseAvatar;

        [Header("Avatar Custom Up to Down (Modular Settings)")]
        [SerializeField] private AvatarCustom[] avatarAccessoriesCustom; // Accessories should be on the top
        [SerializeField] private AvatarCustom[] avatarHeadCustom; // Head should be below Accessories
        [SerializeField] private AvatarCustom[] avatarOutfitCustom; // Outfit should be below Head
        [SerializeField] private AvatarCustom[] avatarTopCustom; // Top should be below Outfit
        [SerializeField] private AvatarCustom[] avatarBottomCustom; // Bottom should be below Top
        [SerializeField] private AvatarCustom[] avatarShoesCustom; // Shoes should be below Bottom

        private AvatarCustom[] GetAvatarCustom(Avatar avatar) // Get the avatar custom array based on the avatar type
        {
            return avatar switch
            {
                Avatar.Accessories => avatarAccessoriesCustom,
                Avatar.Head => avatarHeadCustom,
                Avatar.Outfit => avatarOutfitCustom,
                Avatar.Top => avatarTopCustom,
                Avatar.Bottom => avatarBottomCustom,
                Avatar.Shoes => avatarShoesCustom,
                _ => null,
            };
        }

        public void GetAvatarCustom(Avatar avatar, ref int index, out AvatarCustom avatarCustom) // Get the avatar custom based on the avatar type and index
        {
            avatarCustom = default; // default value if not found
            if (index < defaultIndex)
            {
                index = GetAvatarCustom(avatar).Length - 1; // wrap around to the last index
            }
            else if (index >= GetAvatarCustom(avatar).Length)
            {
                index = defaultIndex; // wrap around to the default index
            }

            if (index == defaultIndex)
            {
                return; // return default value if index is defaultIndex
            }
            avatarCustom = GetAvatarCustom(avatar)[index]; // get the avatar custom at the specified index
        }

        private int[] avatarIndex = null; // array to hold the current index of each avatar type
        private readonly int defaultIndex = -1; // default index for no avatar custom
        private int bitmaskBaseAvatar = default; // current bitmask of base avatar body parts to disable

        public void Start()
        {
            Init(); // Initialize the avatar index array
        }

        private void Init() // Initialize the avatar index array
        {
            avatarIndex = new int[Enum.GetValues(typeof(Avatar)).Length]; // initialize the avatar index array
            Array.Fill(avatarIndex, defaultIndex); // set each avatar index as defaultIndex (-1)
            Debug.Log("Initialization is completed");
        }

        public void SetAvatarCustom(Avatar avatar, int increment, out AvatarCustom avatarCustom) // Set the avatar custom based on the avatar type and increment/decrement the index
        {
            GetAvatarCustom(avatar, ref avatarIndex[(int)avatar], out avatarCustom); // get current avatar custom
            if (!avatarCustom.Equals(default(AvatarCustom))) // if not default, disable the current avatar custom
            {
                bitmaskBaseAvatar &= ~(int)avatarCustom.baseAvatar; // remove the base avatar body parts from the list to disable later
                avatarCustom.renderer.gameObject.SetActive(false); // disable the current avatar custom
                Debug.Log($"Unset Current Avatar Custom {avatar} for {avatarCustom.renderer.name}");
            }
            else
            {
                bitmaskBaseAvatar &= ~(int)(BaseAvatar)(int)avatar; // if default, remove the base avatar body parts from the list to disable later
                Debug.Log($"Unset Current Avatar Custom {avatar} as default");
            }

            // Check for mutual exclusivity between Outfit and Top/Bottom/Shoes
            switch (avatar)
            {
                case Avatar.Outfit:
                    if (avatarIndex[(int)Avatar.Top] != defaultIndex &&
                        avatarIndex[(int)Avatar.Bottom] != defaultIndex &&
                        avatarIndex[(int)Avatar.Shoes] != defaultIndex)
                    {
                        return; // Do not change Outfit if Top, Bottom, Shoes are used
                    }
                    break;
                case Avatar.Top:
                case Avatar.Bottom:
                case Avatar.Shoes:
                    if (avatarIndex[(int)Avatar.Outfit] != defaultIndex)
                    {
                        return; // Do not change Top, Bottom, Shoes if Outfit is used
                    }
                    break;
            }

            avatarIndex[(int)avatar] += increment; // increment or decrement the index
            GetAvatarCustom(avatar, ref avatarIndex[(int)avatar], out avatarCustom); // get the new avatar custom

            if (!avatarCustom.Equals(default(AvatarCustom))) // if not default, enable the new avatar custom
            {
                if ((bitmaskBaseAvatar & (int)avatarCustom.baseAvatar) == 0) // if the base avatar body parts are not already in the list to disable
                {
                    bitmaskBaseAvatar |= (int)avatarCustom.baseAvatar; // add the base avatar body parts to the list to disable later
                }
                avatarCustom.renderer.gameObject.SetActive(true); // enable the new avatar custom
                Debug.Log($"Set New Avatar Custom {avatar} for {avatarCustom.renderer.name}"); // enable the new avatar custom
            }
            else
            {
                bitmaskBaseAvatar &= ~(int)(BaseAvatar)(int)avatar; // if default, remove the base avatar body parts from the list to disable later
                Debug.Log($"Set New Avatar Custom {avatar} as default"); // if default, just log it
            }
            SetBaseAvatar(); // disable the base avatar body parts based on the bitmask list
        }

        private void SetBaseAvatar() // Print all currently disabled base avatar body parts
        {
            foreach (SkinnedMeshRenderer renderer in baseAvatar) // enable all base avatar body parts first
            {
                renderer.gameObject.SetActive(true);
            }

            foreach (BaseAvatar part in Enum.GetValues(typeof(BaseAvatar))) // disable the base avatar body parts based on the combined bitmask
            {
                if ((bitmaskBaseAvatar & (int)part) != 0) // check if the part is in the combined bitmask
                {
                    int index = BitmaskConvertToIndex((int)part); // convert the bitmask to index
                    if (index >= 0 && index < baseAvatar.Length) // check if the index is valid
                    {
                        baseAvatar[index].gameObject.SetActive(false); // disable the base avatar body part
                    }
                }
            }
            Debug.Log("Set Base Avatar Body Parts");
        }
        
        private int BitmaskConvertToIndex(int bitmask) // Convert a bitmask to its corresponding index (0-based)
        {
            int index = 0;
            while (bitmask > 1)
            {
                bitmask >>= 1; // right shift the bitmask
                index++;
            }
            return index;
        }
    }
}