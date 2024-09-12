using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] private Transform wallPrefab;

    public void Clear()
    {
        int l = transform.childCount;
        for (int i = 0; i < l; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void Draw(Cell[,] maze, ushort width, ushort height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cell = maze[x, y];
                var position = new Vector3(x, 0, y);

                // internal walls
                if (cell.HasFlag(Cell.RightWall) && x != width - 1)
                {
                    SpawnWall(position, new Vector3(0.5f, 0, 0), new Vector3(0, 90, 0));
                }

                if (cell.HasFlag(Cell.LowerWall) && y != 0)
                {
                    SpawnWall(position, new Vector3(0, 0, -0.5f), new Vector3(0, 0, 0));
                }

                // edge walls
                if (x == width - 1 && y != 0)
                {
                    SpawnWall(position, new Vector3(0.5f, 0, 0), new Vector3(0, 90, 0));
                }

                if (x == 0 && y != height - 1)
                {
                    SpawnWall(position, new Vector3(-0.5f, 0, 0), new Vector3(0, 90, 0));
                }

                if (y == 0)
                {
                    SpawnWall(position, new Vector3(0, 0, -0.5f), new Vector3(0, 0, 0));
                }

                if (y == height - 1)
                {
                    SpawnWall(position, new Vector3(0, 0, 0.5f), new Vector3(0, 0, 0));
                }
            }
        }
    }

    private void SpawnWall(Vector3 position, Vector3 offset, Vector3 eulerAngles)
    {
        var wall = Instantiate(wallPrefab, transform);
        wall.position = position + offset;
        wall.eulerAngles = eulerAngles;
    }
}