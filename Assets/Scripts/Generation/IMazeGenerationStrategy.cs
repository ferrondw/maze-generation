using System.Collections.Generic;
using UnityEngine;

public interface IMazeGenerationStrategy
{
    // cell[,] is de maze, de stack is de optimal path naar de finish
    (Cell[,], Stack<Vector2Int>) Generate(ushort width, ushort height, int seed = -1);
}