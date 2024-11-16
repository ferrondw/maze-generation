using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RecursiveBacktrackingStrategy : IMazeGenerationStrategy
{
    public (Cell[,], Stack<Vector2Int>) Generate(ushort width, ushort height, int seed = -1)
    {
        var randomInt16 = new System.Random().Next(ushort.MinValue, ushort.MaxValue); // for stashing the seed
        var randomSeed = seed == -1 ? new System.Random(randomInt16) : new System.Random(seed);

        var maze = new Cell[width, height];
        const Cell initial = Cell.RightWall | Cell.LowerWall;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
            }
        }

        var positionStack = new Stack<Vector2Int>();
        var parentMap = new Dictionary<Vector2Int, Vector2Int>();
        var position = new Vector2Int { x = 0, y = height - 1 };

        maze[position.x, position.y] |= Cell.Visited;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = Utilities.GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count <= 0) continue;

            positionStack.Push(current);

            var randomNeighbour = neighbours[randomSeed.Next(0, neighbours.Count)];
            var neighbourPosition = randomNeighbour.Position;
            var (breakPos, wall) = randomNeighbour.WallToBreak;

            maze[breakPos.x, breakPos.y] &= ~wall;
            maze[neighbourPosition.x, neighbourPosition.y] |= Cell.Visited;

            parentMap[neighbourPosition] = current;

            positionStack.Push(neighbourPosition);
        }

        var pathStack = new Stack<Vector2Int>();
        var pathPosition = new Vector2Int { x = width - 1, y = 0 };

        while (pathPosition != new Vector2Int { x = 0, y = height - 1 })
        {
            pathStack.Push(pathPosition);
            pathPosition = parentMap[pathPosition];
        }

        pathStack.Push(new Vector2Int { x = 0, y = height - 1 });
        
        return (maze, pathStack);
    }
}
