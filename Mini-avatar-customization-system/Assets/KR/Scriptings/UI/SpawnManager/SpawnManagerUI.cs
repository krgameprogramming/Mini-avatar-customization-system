namespace KR.Scriptings.UI.Manager
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using KR.Scriptings.Manager;

    public class SpawnManagerUI : MonoBehaviour
    {
        [Header("NPC Spawner UI Settings")]
        [SerializeField] private SpawnManager spawnManager;

        [Header("UI Elements")]
        [SerializeField] private Button spawnNPCButton;
        [SerializeField] private Button despawnNPCButton;
        [SerializeField] private TextMeshProUGUI npcCountText;

        private void Awake()
        {
            spawnNPCButton.onClick.AddListener(() => SpawnNPC(true)); // Add listener to spawn NPC button
            despawnNPCButton.onClick.AddListener(() => SpawnNPC(false)); // Add listener to despawn NPC button
            UpdateNPCCountText(); // Initialize NPC count text
        }

        private void SpawnNPC(bool isSpawning)
        {
            spawnManager.RecallSpawnNPC(isSpawning, out bool succeed, out int activeNPCCount); // Spawn an NPC
            UpdateNPCCountText(isSpawning ? activeNPCCount: activeNPCCount); // Update the NPC count text
        }

        private void UpdateNPCCountText(int npcCount = 0)
        {
            npcCountText.text = $"{npcCount}"; // Update the text to show current NPC count
        }
    }
}