# Mini Avatar Customization System

A lightweight avatar customization system built in Unity3D.  
This was created as part of a hiring test submission for a studio in Kuala Lumpur.  

---

## 🎯 Features

- Swapable avatar parts (e.g. accessories, head, outfit, top, bottom, and shoes)
- Swapable avatar parts having mutual exclusivity for outfit and top/bottom/shoes as packed in the same category
- Simple UI to select parts
- NPC population with random parts selection
- Extensible architecture to add new parts, categories, and features
- Easy to use and integrate into existing projects
note: Bitmask and enum combination for better scalability and performance

---

## ❓ Decisions & Assumptions

Avatar Customization
- Basically this system having base mesh and part categories that can be managed to show/hide based on user customization.
- I chose to implement a simple system that allows users to swap out different parts of an avatar, such as accessories, head, outfit, top, bottom, and shoes.
- Using enum and bitmask combination and computation to manage parts and categories for better scalability and performance.
- Having list for each category to allow easy addition of new parts in the future.
- Each item in the list have two properties: the part itself (Skinned Mesh Renderer) and bitmask selection (enum) for hiding the base mesh when the part is selected.
- Add mutual exclusivity for the outfit category. This is mutually exclusive with the top, bottom, and shoes categories, meaning that selecting an outfit will deselect any top, bottom, or shoes that are currently selected, and vice versa.
- The system is designed to be easily extensible for adding new parts and categories in the future.
- The code is structured in a way that allows for easy addition of categories with minimal code changes, where new parts within the category can be added via the inspector.
- The UI is simple and straightforward, allowing users to easily select and swap out different parts of the avatar.
- The system is designed to be lightweight and efficient, ensuring that it runs smoothly even on lower-end devices.

NPC Population
- I assumed that the NPCs will be populated in the scene using a simple script that randomly selects parts from the available categories to create a unique avatar for each NPC.

note:
- for new objects/models, the parts should be properly rigged and weighted to the base mesh to ensure proper deformation during animations.
- for assigning new objects/models, the parts should be assigned to the correct category list in the AvatarCustomization script to ensure proper functionality (easy add by inspector).


---

## 📁 Code Documentations

note: I used comments in the code for better understanding.
- [`AvatarCustomization.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/Avatar/AvatarCustomization.cs): This script manages the avatar customization system. It handles the selection and swapping of different parts of the avatar, as well as the mutual exclusivity between the outfit and top/bottom/shoes categories. It also provides methods for randomizing the avatar parts, used for NPC population as well. Note: include enums and structs.
- [`AvatarCustomDrawer.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/CustomEditor/AvatarCustomDrawer.cs): This script is a custom property drawer for the AvatarCustomization script. It provides a custom inspector interface for the avatar customization system, allowing users to easily select and swap out different parts of the avatar via buttons.
- [`PlayerMovement.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/Player/PlayerMovement.cs): This script handles the player movement. It allows the player to move around the scene using keyboard inputs. Note: using new Input System package for better scalability and performance, (implemented: Vector 2 Composite WASD keys).
- [`PlayerAnimation.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/Player/PlayerAnimation.cs): This script manages the player animations. It handles the transition between different animation states based on the player's movement, I use a single blend tree for idle and walk/run animations.
- [`CameraController.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/Camera/CameraController.cs): This script controls the camera movement. It allows the camera to follow the player and provides basic camera third person controls and rotating around the player. Note: using Cinemachine package for better scalability and performance. Implemented: Cinemachine FreeLook for third person camera controls with Orbital as Position Control and Rotation Composer as Rotation Control and InputAction to activate the camera rotation (Cinemachine Input Axis Controller) using mouse delta (By Holding Right Mouse Button)
- [`MouseVisibility.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/Misc/MouseVisibility.cs): This script manages the mouse visibility. It allows toggling the mouse cursor visibility and lock state, useful for switching between gameplay and UI interaction. Note: called in the `CameraController.cs` script when holding and releasing the right mouse button to toggle mouse visibility and lock state.
- [`SpawnManager.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/Manager/SpawnManager.cs): This script handles the spawning of NPCs in the scene. It randomly selects parts from the available categories to create a unique avatar for each NPC and spawns them at cached random positions around the player within a defined radius. note: using object pooling, initialize with initial spawn count and hide them all, accessible using UI to toggle visibility. It will enable if current amount is less than initial spawn count and create a new NPC instance from the pool when current amount exceed initial spawn count, Despawn will only hide the NPCs. Accessible via the inspector to set initial spawn count, max spawn count, spawn radius, and NPC prefab.
- [`AvatarCustomizationUI.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/UI/Avatar/AvatarCustomizationUI.cs): This script manages the avatar customization UI. It handles the button interactions for selecting and swapping out different parts of the avatar. It also provides a button to randomize the avatar parts, useful for NPC population as well.
- [`SpawnManagerUI.cs`](Mini-avatar-customization-system/Assets/KR/Scriptings/UI/SpawnManager/SpawnManagerUI.cs): This script manages the spawn manager UI. It handles the button interactions for spawning and despawning NPCs in the scene.