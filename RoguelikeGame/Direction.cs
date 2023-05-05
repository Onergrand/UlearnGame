using System;
using Microsoft.Xna.Framework;

namespace RoguelikeGame;

public enum Direction : byte
{
    North,
    NorthWest,
    NorthEast,
    South,
    SouthEast,
    SouthWest,
    East,
    West
}

public static class DirectionExtensions
{
    public static Direction OppositeDirection(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.NorthWest => Direction.SouthEast,
            Direction.NorthEast => Direction.SouthWest,
            Direction.South => Direction.North,
            Direction.SouthEast => Direction.NorthWest,
            Direction.SouthWest => Direction.NorthEast,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => throw new ArgumentException("Invalid direction", nameof(direction))
        };
    }

    public static Vector2 ConvertToVector(this Direction direction)
    {
        return direction switch
        {
            Direction.North => new Vector2(0, -1),
            Direction.NorthWest => new Vector2((float)-Math.Sqrt(1), (float)-Math.Sqrt(1)),
            Direction.NorthEast => new Vector2((float)Math.Sqrt(1), (float)-Math.Sqrt(1)),
            Direction.South => new Vector2(0, 1),
            Direction.SouthEast => new Vector2((float)Math.Sqrt(1), (float)Math.Sqrt(1)),
            Direction.SouthWest => new Vector2((float)-Math.Sqrt(1), (float)Math.Sqrt(1)),
            Direction.East => new Vector2(1, 0),
            Direction.West => new Vector2(-1, 0),
            _ => throw new ArgumentException("Invalid direction", nameof(direction))
        };
    }
}
