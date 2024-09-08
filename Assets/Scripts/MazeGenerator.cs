using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

// ENORME thanks naar deze blogpost:
// https://weblog.jamisbuck.org/2010/12/27/maze-generation-recursive-backtracking

[Flags] // kom ik ook net achter, maar je kan dus één enum meerdere geven zo
// note for self: |= om toe te voegen, &= ~var om weg te halen
public enum Cell
{
    RightWall = 1,
    LowerWall = 2,
    Visited = 128 // dit gebruik ik alleen voor de generation met de Recursive Backtracking, hoef ik niet op te slaan voor de file
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public (Position, Cell) WallToBreak; // dit moet een tuple zijn omdat ik geen up en left wall heb, dus ik moet zijn neihbouring cell zijn right of down wall hebben
}

public static class MazeGenerator // maze kan MAX 65535 hebben als width en height (wilde eerst gewoon byte gebruiken maar 255 is wel erg klein...)
{

    private static Cell[,] RecursiveBacktracking(Cell[,] maze, byte width, byte height, int seed)
    {
        var random = seed == -1 ? new System.Random() : new System.Random(seed);
        // https://learn.microsoft.com/en-us/dotnet/api/system.random.-ctor?view=net-8.0

        var positionStack = new Stack<Position>(); // voor de backtracking als de cell geen unvisited neighbours meer heeft
        var position = new Position() { X = 0, Y = 0 }; // start altijd linksboven
        maze[position.X, position.Y] |= Cell.Visited;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count <= 0) continue;
            
            positionStack.Push(current);
            var randomNeighbour = neighbours[random.Next(0, neighbours.Count)];

            var neighbourPosition = randomNeighbour.Position;
            (Position breakPos, Cell wall) = randomNeighbour.WallToBreak;
            maze[breakPos.X, breakPos.Y] &= ~wall;
            maze[neighbourPosition.X, neighbourPosition.Y] |= Cell.Visited;
                
            positionStack.Push(neighbourPosition);
        }
        
        return maze;
    }
    
    private static List<Neighbour> GetUnvisitedNeighbours(Position pos, Cell[,] maze, byte width, byte height)
    {
        // indentation goes crazy, ik ga dit trouwens niet allemaal uitleggen want ik ben lui
        // wat dit basically doet is alle kanten checken of zijn neighbour nog niet visited is en niet out of bounds ligt
        // (en dat returnen)
        var neighbours = new List<Neighbour>();

        if (pos.X > 0) // links
        {
            if (!maze[pos.X - 1, pos.Y].HasFlag(Cell.Visited))
            {
                neighbours.Add(new Neighbour()
                {
                    Position = new Position() { X = pos.X - 1, Y = pos.Y },
                    WallToBreak = (new Position() { X = pos.X - 1, Y = pos.Y }, Cell.RightWall)
                });
            }
        }

        if (pos.Y > 0) // onder
        {
            if (!maze[pos.X, pos.Y - 1].HasFlag(Cell.Visited))
            {
                neighbours.Add(new Neighbour()
                {
                    Position = new Position() { X = pos.X, Y = pos.Y - 1 },
                    WallToBreak = (new Position() { X = pos.X, Y = pos.Y }, Cell.LowerWall)
                });
            }
        }

        if (pos.Y < height - 1) // boven
        {
            if (!maze[pos.X, pos.Y + 1].HasFlag(Cell.Visited))
            {
                neighbours.Add(new Neighbour()
                {
                    Position = new Position() { X = pos.X, Y = pos.Y + 1 },
                    WallToBreak = (new Position() { X = pos.X, Y = pos.Y + 1 }, Cell.LowerWall)
                });
            }
        }

        if (pos.X < width - 1) // rechts
        {
            if (!maze[pos.X + 1, pos.Y].HasFlag(Cell.Visited))
            {
                neighbours.Add(new Neighbour()
                {
                    Position = new Position() { X = pos.X + 1, Y = pos.Y },
                    WallToBreak = (new Position() { X = pos.X, Y = pos.Y }, Cell.RightWall)
                });
            }
        }

        return neighbours;
    }

    // hiermee kwam ik trouwens bij de byte en niet uint: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
    public static Cell[,] Generate(byte width, byte height, int seed = -1)
    {
        var maze = new Cell[width, height];

        // initial zodat de algorhytm weer alle muren kan afbreken >:D
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = Cell.RightWall | Cell.LowerWall;
            }
        }

        return RecursiveBacktracking(maze, width, height, seed);
    }
    
    public static byte[] MazeToBytes(Cell[,] maze, byte width, byte height)
    {
        var sb = new StringBuilder();

        // nu heb je [1 byte width][1 byte height]
        sb.Append(Convert.ToString(width, 2).PadLeft(8, '0'));
        sb.Append(Convert.ToString(height, 2).PadLeft(8, '0'));

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                // rechter muur hebben we niet nodig, omdat die altijd gevult word
                if (col != width - 1)
                {
                    sb.Append((maze[col, row] & Cell.RightWall) == Cell.RightWall ? '1' : '0');
                }

                // en de onderste muur hebben hebben we ook niet nodig, bespaart weer width bits!
                if (row != height - 1)
                {
                    sb.Append((maze[col, row] & Cell.LowerWall) == Cell.LowerWall ? '1' : '0');
                }
                
                // door deze checks bespaar je width + height bits
            }
        }

        var binaryString = sb.ToString();
        int padding = 8 - (binaryString.Length % 8);
        if (padding != 8)
        {
            binaryString = binaryString.PadRight(binaryString.Length + padding, '0');
        }

        int numBytes = binaryString.Length / 8;
        byte[] byteArray = new byte[numBytes];

        for (int i = 0; i < numBytes; i++)
        {
            // neem iedere 8 chunk van bits en voeg ze toe aan de byte array
            // dus stel je hebt 0110101101 wordt het [01101011][01...]
            byteArray[i] = Convert.ToByte(binaryString.Substring(8 * i, 8), 2);
        }

        return byteArray;
    }


    public static Cell[,] BytesToMaze(byte[] byteArray)
    {
        // beetje buddel ook in de andere converter, maar ik weet gewoon niet hoe ik bytes direct uit kan lezen
        // dus wordt het maar een intermediate step van bytes naar string lol
        // dit heb ik trouwens direct van google, aint no way dat ik dit maak
        var binary = string.Join(string.Empty, byteArray.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));

        // eerste byte is width, 2de byte is height
        byte width = Convert.ToByte(binary[..8], 2);
        byte height = Convert.ToByte(binary[8..16], 2);
        Debug.Log($"w{width} h{height}");

        var maze = new Cell[width, height];

        int index = 16; // we moeten wel starten bij 16, anders lees je width en height uit als walls

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (col != width - 1)
                {
                    if (binary[index] == '1')
                    {
                        maze[col, row] |= Cell.RightWall;
                    }
                    index++;
                }

                if (row != height - 1)
                {
                    if (binary[index] == '1')
                    {
                        maze[col, row] |= Cell.LowerWall;
                    }
                    index++;
                }
            }
        }

        return maze;
    }

}