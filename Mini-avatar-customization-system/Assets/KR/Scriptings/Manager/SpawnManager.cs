namespace KR.Scriptings.Manager
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawn Manager Settings - NPC will spawn around the player")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float spawnRadius = 3f;

        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private int initialNPCs = 5;
        [SerializeField] private int maxNPCs = 10;

        // for testing purpose to show spawned NPCs in the inspector
        [SerializeField] private List<GameObject> spawnedNPCs = new(); // should be readonly on release
        private readonly Vector2[] cacheRandomPositions = new Vector2[10]; // cache random positions for spawning NPCs
        private int activeNPCCount = 0;

        private void Start()
        {
            InitializeNPCs(); // Initialize NPCs on start
        }

        private void InitializeNPCs()
        {
            // Cache random positions within the spawn radius
            for (int i = 0; i < cacheRandomPositions.Length; i++)
            {
                cacheRandomPositions[i] = new Vector2(UnityEngine.Random.Range(-spawnRadius, spawnRadius), UnityEngine.Random.Range(-spawnRadius, spawnRadius));
            }

            spawnedNPCs.Capacity = maxNPCs; // Set the capacity of the list to the maximum NPCs
            initialNPCs = Mathf.Min(initialNPCs, maxNPCs); // Ensure initial NPCs do not exceed maximum

            // Spawn NPCs up to the maximum limit
            for (int i = 0; i < initialNPCs; i++)
            {
                SpawnNPC(init: true, out _); // Spawn NPC without logging
            }
        }

        private Vector3 GenerateRandomPosition()
        {
            Vector2 randomPos = cacheRandomPositions[UnityEngine.Random.Range(0, cacheRandomPositions.Length)]; // Get a random cached position
            return new Vector3(playerTransform.position.x + randomPos.x, playerTransform.position.y, playerTransform.position.z + randomPos.y); // Return the position relative to the player
        }

        public void RecallSpawnNPC(bool isSpawning, out bool succeed, out int activeNPCCount)
        {
            succeed = false; // Default to false
            activeNPCCount = this.activeNPCCount;
            switch (isSpawning)
            {
                case true:
                    if (activeNPCCount == maxNPCs) // Check if all NPCs are already active
                    {
                        Debug.LogWarning("All NPCs are already active. Cannot spawn more.");
                        return;
                    }

                    if (activeNPCCount < spawnedNPCs.Count && !spawnedNPCs[activeNPCCount].activeInHierarchy) // Check for inactive NPCs to respawn
                    {
                        succeed = true; // Set succeed to true
                    }
                    else
                    {
                        // If no inactive NPCs are found, spawn a new one
                        SpawnNPC(init: false, out succeed); // Spawn NPC with logging
                        // succeed is set within the SpawnNPC method to true if spawning was successful
                    }

                    if (succeed) // If spawning was successful
                    {
                        spawnedNPCs[activeNPCCount].transform.position = GenerateRandomPosition(); // Move the NPC to a new random position
                        spawnedNPCs[activeNPCCount].SetActive(true); // Reactivate the NPC
                        Debug.Log($"Respawned {spawnedNPCs[activeNPCCount].name}. Total Active NPCs: {activeNPCCount}");
                        activeNPCCount = ++this.activeNPCCount; // Increment the active NPC count
                    }
                    break;
                case false:
                    if (activeNPCCount == 0) // Check if there are no active NPCs to despawn
                    {
                        Debug.LogWarning("No active NPCs to despawn.");
                        return;
                    }
                    
                    activeNPCCount = --this.activeNPCCount; // Decrement the active NPC count
                    if (activeNPCCount >= 0 && spawnedNPCs[activeNPCCount].activeInHierarchy) // Check if the last active NPC is indeed active
                    {
                        succeed = true; // Set succeed to true
                        spawnedNPCs[activeNPCCount].SetActive(false); // Deactivate the NPC
                        Debug.Log($"Despawned {spawnedNPCs[activeNPCCount].name}. Total Active NPCs: {activeNPCCount}");
                    }
                    break;
            }
        }

        private void SpawnNPC(bool init, out bool succeed)
        {
            succeed = false; // Default to false
            if (spawnedNPCs.Count >= maxNPCs) // Check if maximum NPC limit is reached
            {
                Debug.LogWarning("Maximum NPC limit reached. Cannot spawn more NPCs.");
                return;
            }

            GameObject newNPC = Instantiate(npcPrefab, GenerateRandomPosition(), Quaternion.identity); // Instantiate NPC at a random position around the player
            spawnedNPCs.Add(newNPC); // Add the new NPC to the list
            spawnedNPCs[^1].name = $"NPC_{spawnedNPCs.Count}"; // Name the NPC based on the current count
            spawnedNPCs[^1].SetActive(false); // Ensure the NPC is inactive
            if (spawnedNPCs.Count == maxNPCs)
            {
                spawnedNPCs.TrimExcess(); // Optimize memory usage
                Debug.Log("Reached maximum NPC capacity. Trimmed excess memory.");
            }
            succeed = true; // Set succeed to true
            Debug.Log($"Spawned {newNPC.name} at {newNPC.transform.position}. Total NPCs: {spawnedNPCs.Count}");
        }
    }
}