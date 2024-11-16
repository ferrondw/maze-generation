using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] private Mesh wallMesh;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private float wallHeight;

    private readonly List<Matrix4x4> wallMatrices = new();

    public void Clear()
    {
        wallMatrices.Clear();
    }

    public void Draw(Cell[,] maze)
    {
        Clear();

        var width = maze.GetLength(0);
        var height = maze.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cell = maze[x, y];
                var position = new Vector3(x, 0, y);

                // internal walls
                if (cell.HasFlag(Cell.RightWall) && x != width - 1)
                {
                    AddWall(position, new Vector3(0.5f, wallHeight, 0), Quaternion.Euler(0, 180, 0));
                }

                if (cell.HasFlag(Cell.LowerWall) && y != 0)
                {
                    AddWall(position, new Vector3(0, wallHeight, -0.5f), Quaternion.Euler(0, 90, 0));
                }

                // edge walls
                if (x == width - 1 && y != 0)
                {
                    AddWall(position, new Vector3(0.5f, wallHeight, 0), Quaternion.Euler(0, 180, 0));
                }

                if (x == 0 && y != height - 1)
                {
                    AddWall(position, new Vector3(-0.5f, wallHeight, 0), Quaternion.Euler(0, 180, 0));
                }

                if (y == 0)
                {
                    AddWall(position, new Vector3(0, wallHeight, -0.5f), Quaternion.Euler(0, 90, 0));
                }

                if (y == height - 1)
                {
                    AddWall(position, new Vector3(0, wallHeight, 0.5f), Quaternion.Euler(0, 90, 0));
                }
            }
        }
    }

    private void AddWall(Vector3 position, Vector3 offset, Quaternion rotation)
    {
        Matrix4x4 matrix = Matrix4x4.TRS(position + offset, rotation, Vector3.one * 0.5f);
        wallMatrices.Add(matrix);
    }

    private void Update()
    {
        DrawWalls();
    }

    private void DrawWalls()
    {
        const int batchSize = 1023;
        for (int i = 0; i < wallMatrices.Count; i += batchSize)
        {
            int count = Mathf.Min(batchSize, wallMatrices.Count - i);
            Graphics.DrawMeshInstanced(wallMesh, 0, wallMaterial, wallMatrices.GetRange(i, count), null, ShadowCastingMode.On, true, 0, null, LightProbeUsage.Off, null);
        }
    }
}
