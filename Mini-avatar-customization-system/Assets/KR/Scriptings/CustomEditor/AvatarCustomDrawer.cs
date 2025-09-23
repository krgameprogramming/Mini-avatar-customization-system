namespace KR.Scriptings.CustomEditor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Linq; // Needed for the Linq methods at the end
    using KR.Scriptings.Avatar;

    [CustomPropertyDrawer(typeof(AvatarCustom))]
    public class AvatarCustomDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Find the properties within the struct
            SerializedProperty rendererProp = property.FindPropertyRelative("renderer");
            SerializedProperty baseAvatarProp = property.FindPropertyRelative("baseAvatar");

            // Calculate a rect for the renderer field
            Rect rendererRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rendererRect, rendererProp, new GUIContent("Renderer"));
            
            // Calculate a rect for the enum dropdown on the next line
            Rect enumRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

            // This is the core logic for the dropdown
            // Get the current enum value as an int
            int baseAvatarValue = baseAvatarProp.intValue;

            // Get the name of all options that are currently selected
            string[] selectedNames = Enum.GetValues(typeof(BaseAvatar))
                .Cast<BaseAvatar>()
                .Where(flag => (baseAvatarValue & (int)flag) != 0)
                .Select(flag => Enum.GetName(typeof(BaseAvatar), flag))
                .ToArray();

            // Create the label for the dropdown. Show "None" if nothing is selected.
            string buttonLabel = selectedNames.Length > 0 ? string.Join(", ", selectedNames) : "None";

            // Draw the dropdown button
            if (EditorGUI.DropdownButton(enumRect, new GUIContent(buttonLabel), FocusType.Keyboard))
            {
                // Create a menu for the dropdown
                GenericMenu menu = new GenericMenu();
                
                // Populate the menu with toggles for each enum option
                foreach (var enumValue in Enum.GetValues(typeof(BaseAvatar)))
                {
                    BaseAvatar flag = (BaseAvatar)enumValue;
                    bool isSelected = (baseAvatarValue & (int)flag) != 0;

                    menu.AddItem(new GUIContent(Enum.GetName(typeof(BaseAvatar), flag)), isSelected, () => {
                        // This action runs when a menu item is clicked
                        baseAvatarProp.intValue ^= (int)flag; // XOR to toggle the bit
                        baseAvatarProp.serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            }

            EditorGUI.EndProperty();
        }

        // You still need to calculate the correct height
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Height for the renderer field + the enum dropdown
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
    }
}