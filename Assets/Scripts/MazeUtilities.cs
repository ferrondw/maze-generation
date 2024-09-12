using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MazeUtilities
{
    public static List<Neighbour> GetUnvisitedNeighbours(Vector2Int pos, Cell[,] maze, ushort width, ushort height)
    {
        var neighbours = new List<Neighbour>();

        if (pos.x > 0 && !maze[pos.x - 1, pos.y].HasFlag(Cell.Visited))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Vector2Int { x = pos.x - 1, y = pos.y },
                WallToBreak = (new Vector2Int { x = pos.x - 1, y = pos.y }, Cell.RightWall)
            });
        }

        if (pos.y > 0 && !maze[pos.x, pos.y - 1].HasFlag(Cell.Visited))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Vector2Int { x = pos.x, y = pos.y - 1 },
                WallToBreak = (new Vector2Int { x = pos.x, y = pos.y }, Cell.LowerWall)
            });
        }

        if (pos.y < height - 1 && !maze[pos.x, pos.y + 1].HasFlag(Cell.Visited))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Vector2Int { x = pos.x, y = pos.y + 1 },
                WallToBreak = (new Vector2Int { x = pos.x, y = pos.y + 1 }, Cell.LowerWall)
            });
        }

        if (pos.x < width - 1 && !maze[pos.x + 1, pos.y].HasFlag(Cell.Visited))
        {
            neighbours.Add(new Neighbour
            {
                Position = new Vector2Int { x = pos.x + 1, y = pos.y },
                WallToBreak = (new Vector2Int { x = pos.x, y = pos.y }, Cell.RightWall)
            });
        }

        return neighbours;
    }

    public static byte[] MazeToBytes(Cell[,] maze, ushort width, ushort height)
    {
        var sb = new StringBuilder();
        sb.Append(Convert.ToString(width, 2).PadLeft(16, '0'));
        sb.Append(Convert.ToString(height, 2).PadLeft(16, '0'));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x != width - 1)
                {
                    sb.Append((maze[x, y] & Cell.RightWall) == Cell.RightWall ? '1' : '0');
                }

                if (y != 0)
                {
                    sb.Append((maze[x, y] & Cell.LowerWall) == Cell.LowerWall ? '1' : '0');
                }
            }
        }

        var binaryString = sb.ToString();
        var padding = 8 - binaryString.Length % 8;
        if (padding != 8)
        {
            binaryString = binaryString.PadRight(binaryString.Length + padding, '0');
        }

        var numBytes = binaryString.Length / 8;
        var byteArray = new byte[numBytes];

        for (int i = 0; i < numBytes; i++)
        {
            byteArray[i] = Convert.ToByte(binaryString.Substring(8 * i, 8), 2);
        }

        return byteArray;
    }


    public static Cell[,] BytesToMaze(IEnumerable<byte> byteArray)
    {
        var binary = string.Join(string.Empty, byteArray.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

        var width = Convert.ToUInt16(binary[..16], 2);
        var height = Convert.ToUInt16(binary[16..32], 2);
        var maze = new Cell[width, height];
        var index = 16;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (col != width - 1)
                {
                    if (binary[index] == '1') maze[col, row] |= Cell.RightWall;
                    index++;
                }

                if (row != 0)
                {
                    if (binary[index] == '1') maze[col, row] |= Cell.LowerWall;
                    index++;
                }
            }
        }

        return maze;
    }
}