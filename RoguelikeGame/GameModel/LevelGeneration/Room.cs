using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameModel.LevelGeneration;

public delegate void OutRoomBoundsDelegate();

public class Room
{
    public event OutRoomBoundsDelegate PlayerIsOutsideRoom;
    
    public int Length { get; }
    public int Width { get; }
    public Point TopLeftCorner { get; }
    
    public readonly Dictionary<Direction, Room> Neighbours;

    public Room(Point topLeftCorner, int length, int width)
    {
        TopLeftCorner = topLeftCorner;
        Neighbours = new Dictionary<Direction, Room>();
        Length = length;
        Width = width;
    }
    
    public bool TryCreateNeighbour(Direction direction)
    {
        var neighbour = new Room(GetNextRoomPosition(direction), Length, Width);

        if (neighbour.TopLeftCorner.X is > 82 or < 16 || neighbour.TopLeftCorner.Y is > 85 or < 13)
            return false;
            
        
        neighbour.Neighbours[direction.OppositeDirection()] = this;
        
        Neighbours[direction] = neighbour;
        return true;
    }

    private Point GetNextRoomPosition(Direction direction)
    {
        var nextPoint = TopLeftCorner;
        switch (direction)
        {
            case Direction.North:
                nextPoint -= new Point(0, Width);
                break;
                
            case Direction.West:
                nextPoint -= new Point(Length, 0);
                break;
                
            case Direction.East:
                nextPoint += new Point(Length, 0);
                break;
                
            case Direction.South:
                nextPoint += new Point(0, Width);
                break;
        }

        return nextPoint;
    }
    
    public bool IsPositionInRoomBounds(Vector2 position)
    {
        var room = this;
        var topLeft = (new Vector2(room.TopLeftCorner.X, room.TopLeftCorner.Y) - Vector2.One) * 50 - new Vector2(2, 2);
        var bottomRight = new Vector2(topLeft.X + room.Length * 50, topLeft.Y + room.Width * 50) + new Vector2(2, 2);

        if (position.X < topLeft.X || position.Y < topLeft.Y || position.X > bottomRight.X || position.Y > bottomRight.Y) 
            return false;
        
        return true;
    }
    
    public void IsPlayerInRoomBounds(Vector2 position)
    {
        var room = this;
        var topLeft = new Vector2((room.TopLeftCorner.X - 1) * 50, (room.TopLeftCorner.Y - 1) * 50);
        var bottomRight = new Vector2(topLeft.X + (room.Length + 1) * 50, topLeft.Y + (room.Width + 1) * 50);

        if (position.X < topLeft.X || position.Y < topLeft.Y || position.X > bottomRight.X || position.Y > bottomRight.Y)
            PlayerIsOutsideRoom!();
    }
}