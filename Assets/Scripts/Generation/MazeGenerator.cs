using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    public static (Cell[,], Stack<Vector2Int>) Generate(ushort width, ushort height, IMazeGenerationStrategy strategy, int seed = -1)
    {
        return strategy.Generate(width, height, seed);
    }
}