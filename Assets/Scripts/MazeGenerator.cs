using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    public static Cell[,] Generate(ushort width, ushort height, int seed = -1)
    {
        var maze = new Cell[width, height];
        const Cell initial = Cell.RightWall | Cell.LowerWall;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
            }
        }

        return RecursiveBacktracking(maze, width, height, seed);
    }
    
    private static Cell[,] RecursiveBacktracking(Cell[,] maze, ushort width, ushort height, int seed)
    {
        var randomSeed = seed == -1 ? new System.Random() : new System.Random(seed);

        var positionStack = new Stack<Vector2Int>();
        var position = new Vector2Int { x = 0, y = 0 };
        
        maze[position.x, position.y] |= Cell.Visited;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = MazeUtilities.GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count <= 0) continue;

            positionStack.Push(current);
            
            var randomNeighbour = neighbours[randomSeed.Next(0, neighbours.Count)];
            var neighbourPosition = randomNeighbour.Position;
            var (breakPos, wall) = randomNeighbour.WallToBreak;
            
            maze[breakPos.x, breakPos.y] &= ~wall;
            maze[neighbourPosition.x, neighbourPosition.y] |= Cell.Visited;

            positionStack.Push(neighbourPosition);
        }

        return maze;
    }
}