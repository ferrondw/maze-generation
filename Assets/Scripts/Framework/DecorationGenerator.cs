using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DecorationGenerator : MonoBehaviour
{
    [SerializeField] private Mesh flowerMesh;
    [SerializeField] private Mesh trunkMesh;
    [SerializeField] private Mesh leavesMesh;
    [SerializeField] private Mesh leavesOverlayMesh;
    [SerializeField] private Material flowerMaterial;
    [SerializeField] private Material trunkMaterial;
    [SerializeField] private Material leavesStaticMaterial;
    [SerializeField] private Material leavesDynamicMaterial;
    [SerializeField] private float minDensity;
    [SerializeField] private float maxDensity;
    [SerializeField] private float rotation;
    [SerializeField] private float height;
    [SerializeField] private float noiseScale;
    [SerializeField] private float treeChance = 0.3f;
    [SerializeField] private Vector3 relativeLeavePosition = new(0, 0.1f, 0);
    [SerializeField] private float trunkHeight = 0.95f;
    [SerializeField] private float trunkScale = 0.5f;
    [SerializeField] private float leavesScale = 0.5f;
    [SerializeField] private float leavesOverlayScale = 0.42f;
    [SerializeField] private float flowerScale = 0.2f;
    [SerializeField] private float randomTreeOffset = 0.3f;
    [SerializeField] private float randomFlowerOffset = 0.5f;

    private readonly List<Matrix4x4> flowerMatrices = new();
    private readonly List<Matrix4x4> trunkMatrices = new();
    private readonly List<Matrix4x4> leavesStaticMatrices = new();
    private readonly List<Matrix4x4> leavesDynamicMatrices = new();

    public void Generate(ushort mazeWidth, ushort mazeHeight, ushort padding, ushort inlinePadding)
    {
        Clear();

        var seed = new System.Random().Next();
        for (int x = 0 - padding; x < mazeWidth + padding; x++)
        {
            for (int z = 0 - padding; z < mazeHeight + padding; z++)
            {
                float noiseValue = Mathf.PerlinNoise(seed + x / noiseScale, seed + z / noiseScale);
                noiseValue = Mathf.Clamp(noiseValue, minDensity, maxDensity);

                float flowerDensity = noiseValue * 10f;
                int flowerCount = Mathf.RoundToInt(flowerDensity);

                if (Random.Range(0f, 1f) < treeChance && (x + inlinePadding < 0 || x - inlinePadding > mazeWidth || z + inlinePadding < 0 || z - inlinePadding > mazeHeight))
                {
                    float offsetX = Random.Range(-randomTreeOffset, randomTreeOffset);
                    float offsetZ = Random.Range(-randomTreeOffset, randomTreeOffset);
                    Vector3 basePosition = new Vector3(x + offsetX, trunkHeight, z + offsetZ);

                    trunkMatrices.Add(Matrix4x4.TRS(basePosition, Quaternion.identity, Vector3.one * trunkScale));
                    leavesStaticMatrices.Add(Matrix4x4.TRS(basePosition, Quaternion.identity, Vector3.one * leavesScale));
                    
                    Vector3 leavesOverlayPosition = basePosition + relativeLeavePosition;
                    leavesDynamicMatrices.Add(Matrix4x4.TRS(leavesOverlayPosition, Quaternion.identity, Vector3.one * leavesOverlayScale));
                }

                for (int i = 0; i < flowerCount; i++)
                {
                    float offsetX = Random.Range(-randomFlowerOffset, randomFlowerOffset);
                    float offsetZ = Random.Range(-randomFlowerOffset, randomFlowerOffset);
                    Vector3 flowerPosition = new Vector3(x + offsetX, height, z + offsetZ);
                    Quaternion flowerRotation = Quaternion.Euler(rotation, Random.Range(0, 180), 0);

                    flowerMatrices.Add(Matrix4x4.TRS(flowerPosition, flowerRotation, Vector3.one * flowerScale));
                }
            }
        }
    }

    private void Update()
    {
        DrawMeshInChunks(flowerMesh, flowerMaterial, flowerMatrices, ShadowCastingMode.Off);
        DrawMeshInChunks(trunkMesh, trunkMaterial, trunkMatrices, ShadowCastingMode.Off);
        DrawMeshInChunks(leavesMesh, leavesStaticMaterial, leavesStaticMatrices, ShadowCastingMode.Off);
        DrawMeshInChunks(leavesOverlayMesh, leavesDynamicMaterial, leavesDynamicMatrices, ShadowCastingMode.Off);
    }

    private static void DrawMeshInChunks(Mesh mesh, Material material, List<Matrix4x4> matrices, ShadowCastingMode shadowCastingMode)
    {
        const int batchSize = 1023;
        for (int i = 0; i < matrices.Count; i += batchSize)
        {
            int count = Mathf.Min(batchSize, matrices.Count - i);
            Graphics.DrawMeshInstanced(
                mesh,
                0,
                material,
                matrices.GetRange(i, count),
                null,
                shadowCastingMode,
                false
            );
        }
    }
    
    public void Clear()
    {
        flowerMatrices.Clear();
        trunkMatrices.Clear();
        leavesStaticMatrices.Clear();
        leavesDynamicMatrices.Clear();
    }
}
