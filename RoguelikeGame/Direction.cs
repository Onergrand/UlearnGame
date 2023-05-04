using System;

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
}
