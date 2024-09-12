using System;

[Flags]
public enum Cell
{
    RightWall = 1,
    LowerWall = 2,
    Visited = 4
}