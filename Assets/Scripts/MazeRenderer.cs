using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField] private Transform wallPrefab;
    [SerializeField] private CameraScaler scaler;

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
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width * 0.5f + i, 0, -height * 0.5f + j); // centered coords

                if (cell.HasFlag(Cell.RightWall) && i != width - 1)
                {
                    var wall = Instantiate(wallPrefab, transform);
                    wall.position = position + new Vector3(0.5f, 0, 0);
                    wall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (cell.HasFlag(Cell.LowerWall) && j != 0)
                {
                    var wall = Instantiate(wallPrefab, transform);
                    wall.position = position + new Vector3(0, 0, -0.5f);
                    wall.eulerAngles = new Vector3(0, 0, 0);
                }

                // EDGE WALLS

                //rechts
                if (i == width - 1 && j != 0)
                {
                    var wall = Instantiate(wallPrefab, transform);
                    wall.position = position + new Vector3(0.5f, 0, 0);
                    wall.eulerAngles = new Vector3(0, 90, 0);
                }

                //links
                if (i == 0 && j != height - 1)
                {
                    var wall = Instantiate(wallPrefab, transform);
                    wall.position = position + new Vector3(-0.5f, 0, 0);
                    wall.eulerAngles = new Vector3(0, 90, 0);
                }

                // onder
                if (j == 0)
                {
                    var wall = Instantiate(wallPrefab, transform);
                    wall.position = position + new Vector3(0, 0, -0.5f);
                    wall.eulerAngles = new Vector3(0, 0, 0);
                }

                //boven
                if (j == height - 1)
                {
                    var wall = Instantiate(wallPrefab, transform);
                    wall.position = position + new Vector3(0, 0, 0.5f);
                    wall.eulerAngles = new Vector3(0, 0, 0);
                }
            }
        }

        scaler.ScaleCameraToMaze(width, height);
    }
}