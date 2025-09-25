namespace KR.Scriptings.Performance
{
    using UnityEngine;
    using System.Collections.Generic;

    public class SkinnedMeshCombiner : MonoBehaviour
    {
        private void Start()
        {
            CombineSkinnedMeshes();
        }

        public void CombineSkinnedMeshes() // Call this method to combine skinned meshes and destroy originals
        {
            // Find all SkinnedMeshRenderers on this object and its children
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (renderers == null || renderers.Length <= 1)
            {
                Debug.LogWarning("No child SkinnedMeshRenderers found to combine!");
                return;
            }

            // The parent object's renderer, where the new combined mesh will go
            if (!TryGetComponent<SkinnedMeshRenderer>(out var newRenderer))
            {
                newRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
            }

            List<CombineInstance> combineInstances = new();
            List<Material> materials = new();
            List<Transform> allBones = new();
            Dictionary<string, Transform> boneMap = new();

            // Pass 1: Collect unique bones and materials, and create CombineInstances
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                if (renderer == newRenderer) continue;

                // Collect all unique bones from the current renderer
                foreach (Transform bone in renderer.bones)
                {
                    if (!boneMap.ContainsKey(bone.name))
                    {
                        boneMap.Add(bone.name, bone);
                        allBones.Add(bone);
                    }
                }

                // Collect all materials from the current renderer
                materials.AddRange(renderer.sharedMaterials);

                // Create a CombineInstance for each sub-mesh
                for (int i = 0; i < renderer.sharedMesh.subMeshCount; i++)
                {
                    CombineInstance combine = new()
                    {
                        mesh = renderer.sharedMesh,
                        subMeshIndex = i
                    };
                    // Skinned meshes don't need to specify a transform matrix for combining,
                    // as their final position is determined by the skeleton.
                    combineInstances.Add(combine);
                }

                // Disable the old renderer to hide it
                renderer.enabled = false;
            }

            // Create the new combined mesh
            Mesh combinedMesh = new();
            combinedMesh.CombineMeshes(combineInstances.ToArray(), false, false); // mergeSubMeshes is false to keep materials separate

            // Pass 2: Remap bone weights and bind poses for the combined mesh
            BoneWeight[] combinedBoneWeights = new BoneWeight[combinedMesh.vertexCount];
            Matrix4x4[] combinedBindPoses = new Matrix4x4[allBones.Count];
            int vertexOffset = 0;

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                if (renderer == newRenderer) continue;

                // Remap bone weights from the original mesh to the new bone list
                BoneWeight[] originalWeights = renderer.sharedMesh.boneWeights;
                Transform[] originalBones = renderer.bones;

                for (int i = 0; i < originalWeights.Length; i++)
                {
                    BoneWeight oldWeight = originalWeights[i];
                    BoneWeight newWeight = new()
                    {
                        boneIndex0 = allBones.IndexOf(originalBones[oldWeight.boneIndex0]),
                        weight0 = oldWeight.weight0,

                        boneIndex1 = allBones.IndexOf(originalBones[oldWeight.boneIndex1]),
                        weight1 = oldWeight.weight1,

                        boneIndex2 = allBones.IndexOf(originalBones[oldWeight.boneIndex2]),
                        weight2 = oldWeight.weight2,

                        boneIndex3 = allBones.IndexOf(originalBones[oldWeight.boneIndex3]),
                        weight3 = oldWeight.weight3
                    };

                    combinedBoneWeights[vertexOffset + i] = newWeight;
                }

                // Collect and remap the bind poses
                for (int i = 0; i < originalBones.Length; i++)
                {
                    int boneIndex = allBones.IndexOf(originalBones[i]);
                    if (boneIndex != -1)
                    {
                        combinedBindPoses[boneIndex] = renderer.sharedMesh.bindposes[i];
                    }
                }

                vertexOffset += originalWeights.Length;
            }

            // Assign all the new properties to the final renderer
            newRenderer.sharedMesh = combinedMesh;
            newRenderer.sharedMaterials = materials.ToArray();
            newRenderer.bones = allBones.ToArray();
            newRenderer.sharedMesh.boneWeights = combinedBoneWeights;
            newRenderer.sharedMesh.bindposes = combinedBindPoses;
            newRenderer.sharedMesh.RecalculateBounds();

            // **NEW ANIMATION FIX**
            // Assign the new bones array to the renderer
            Animator animator = GetComponentInParent<Animator>();

            // Assign the root bone for the new renderer, which is the animator's root bone
            if (animator != null && animator.avatar != null && animator.avatar.isHuman)
            {
                // If the animator uses a Humanoid rig, find the hips bone
                newRenderer.rootBone = animator.GetBoneTransform(HumanBodyBones.Hips);
                animator.Rebind(); // Rebind the animator to ensure it recognizes the new bone structure
                animator.Update(0f); // Update the animator to apply the changes
                Debug.Log("Assigned root bone: " + newRenderer.rootBone.name);
            }
            else
            {
                Debug.LogError("Animator or Avatar is missing!");
            }

            // Clean up the original child objects
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                if (renderer != newRenderer)
                {
                    Destroy(renderer.gameObject);
                }
            }

            Debug.Log("Skinned meshes combined successfully!");
        }

        // Advanced version with toggle support (commented out for now)
        // private SkinnedMeshRenderer combinedRenderer;
        // private readonly Dictionary<string, int> bodyPartToSubmesh = new();
        // private Material transparentMaterial;

        // public void CombineSkinnedMeshes()
        // {
        //     // Find all SkinnedMeshRenderers on this object and its children
        //     SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        //     if (renderers == null || renderers.Length <= 1)
        //     {
        //         Debug.LogWarning("No child SkinnedMeshRenderers found to combine!");
        //         return;
        //     }

        //     // Create transparent material for toggling
        //     transparentMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        //     transparentMaterial.color = new Color(0, 0, 0, 0);
        //     transparentMaterial.renderQueue = 3000; // Transparent

        //     // Combined renderer setup
        //     if (!TryGetComponent(out combinedRenderer))
        //         combinedRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();

        //     List<CombineInstance> combineInstances = new();
        //     List<Material> materials = new();
        //     List<Transform> allBones = new();
        //     Dictionary<string, Transform> boneMap = new();

        //     int currentSubmesh = 0;

        //     foreach (SkinnedMeshRenderer renderer in renderers)
        //     {
        //         if (renderer == combinedRenderer) continue;

        //         // Collect unique bones
        //         foreach (Transform bone in renderer.bones)
        //         {
        //             if (!boneMap.ContainsKey(bone.name))
        //             {
        //                 boneMap.Add(bone.name, bone);
        //                 allBones.Add(bone);
        //             }
        //         }

        //         // Collect materials and record part-to-submesh mapping
        //         foreach (var mat in renderer.sharedMaterials)
        //         {
        //             materials.Add(mat);
        //             bodyPartToSubmesh[renderer.name] = currentSubmesh; // map renderer name to submesh index
        //             currentSubmesh++;
        //         }

        //         // Create combine instances
        //         for (int i = 0; i < renderer.sharedMesh.subMeshCount; i++)
        //         {
        //             CombineInstance combine = new()
        //             {
        //                 mesh = renderer.sharedMesh,
        //                 subMeshIndex = i
        //             };
        //             combineInstances.Add(combine);
        //         }

        //         // Hide old renderer but keep GameObject (in case needed)
        //         renderer.enabled = true; // keep enabled for potential future use
        //     }

        //     // Create combined mesh
        //     Mesh combinedMesh = new();
        //     combinedMesh.CombineMeshes(combineInstances.ToArray(), false, false);

        //     // Assign renderer data
        //     combinedRenderer.sharedMesh = combinedMesh;
        //     combinedRenderer.sharedMaterials = materials.ToArray();
        //     combinedRenderer.bones = allBones.ToArray();
        //     combinedRenderer.rootBone = GetComponentInParent<Animator>().GetBoneTransform(HumanBodyBones.Hips);
        //     combinedMesh.RecalculateBounds();

        //     Debug.Log("Skinned meshes combined successfully with toggle support!");
        // }

        // /// <summary>
        // /// Toggle a body part on/off by swapping material with transparent one.
        // /// </summary>
        // public void TogglePart(string partName, bool enabled)
        // {
        //     if (!bodyPartToSubmesh.TryGetValue(partName, out int submeshIndex))
        //     {
        //         Debug.LogWarning("No body part found with name: " + partName);
        //         return;
        //     }

        //     var mats = combinedRenderer.sharedMaterials;
        //     mats[submeshIndex] = enabled ? mats[submeshIndex] : transparentMaterial;
        //     combinedRenderer.sharedMaterials = mats;
        // }
    }
}