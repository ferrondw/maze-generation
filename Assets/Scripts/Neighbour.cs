using UnityEngine;

public struct Neighbour
{
    public Vector2Int Position;
    public (Vector2Int, Cell) WallToBreak;
}