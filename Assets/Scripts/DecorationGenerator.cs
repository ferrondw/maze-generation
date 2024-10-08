using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DecorationGenerator : MonoBehaviour
{
    [SerializeField] private GameObject flowerPrefab;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private float minDensity;
    [SerializeField] private float maxDensity;
    [SerializeField] private float rotation;
    [SerializeField] private float height;
    [SerializeField] private float noiseScale;
    [SerializeField] private float treeChance = 0.3f;
    
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
                    float offsetX = Random.Range(-0.3f, 0.3f);
                    float offsetZ = Random.Range(-0.3f, 0.3f);

                    Vector3 treePosition = new Vector3(x + offsetX, 0.95f, z + offsetZ);
                    GameObject tree = Instantiate(treePrefab, treePosition, quaternion.identity);
                    tree.transform.eulerAngles = new Vector3(7, 0, 0);
                    tree.transform.parent = transform;
                }

                for (int i = 0; i < flowerCount; i++)
                {
                    float offsetX = Random.Range(-0.5f, 0.5f);
                    float offsetZ = Random.Range(-0.5f, 0.5f);
                
                    Vector3 flowerPosition = new Vector3(x + offsetX, height, z + offsetZ);
                    GameObject flower = Instantiate(flowerPrefab, flowerPosition, quaternion.identity);
                    flower.transform.eulerAngles = new Vector3(rotation, Random.Range(0, 180), 0);
                    flower.transform.parent = transform;
                }
            }
        }
    }

    private void Clear()
    {
        int l = transform.childCount;
        for (int i = 0; i < l; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
