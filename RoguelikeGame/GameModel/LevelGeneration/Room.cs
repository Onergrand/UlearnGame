using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameModel.LevelGeneration;

public delegate void OutRoomBoundsDelegate();

public class Room
{
    public event OutRoomBoundsDelegate PlayerIsOutsideRoom;
    
    public int Length { get; }
    public int Breadth { get; }
    public Point TopLeftCorner { get; }
    public int RoomsLeft { get; }
    //public readonly  RoomMap
    public readonly Dictionary<Direction, Room> Neighbours;
    
    private readonly List<Direction> _possibleDirections = new() { Direction.North, Direction.East, Direction.West, Direction.South };

    public Room(Point topLeftCorner, int length, int breadth, int roomsLeft)
    {
        TopLeftCorner = topLeftCorner;
        Neighbours = new Dictionary<Direction, Room>();
        Length = length;
        Breadth = breadth;
        RoomsLeft = roomsLeft;
    }

    private Room(Point topLeftCorner, Dictionary<Direction, Room> neighbours, int length, int breadth, int roomsLeft)
    {
        TopLeftCorner = topLeftCorner;
        Length = length;
        Breadth = breadth;
        RoomsLeft = roomsLeft;
        
        Neighbours = neighbours;
        _possibleDirections = _possibleDirections.Except(Neighbours.Keys).ToList();
    }

    public void CreateNeighbours(Level level)
    {
        var rnd = new Random();
        var roomsAmount = rnd.Next(0, RoomsLeft > _possibleDirections.Count ? _possibleDirections.Count : RoomsLeft);

        for (var i = 0; i < roomsAmount; i++)
        {
            var neighbourDirectionIndex = rnd.Next(_possibleDirections.Count);
            var neighbourDirection = _possibleDirections[neighbourDirectionIndex];
            _possibleDirections.RemoveAt(neighbourDirectionIndex);


            var nextPoint = TopLeftCorner;
            switch (neighbourDirection)
            {
                case Direction.North:
                    nextPoint -= new Point(0, Breadth);
                    break;
                
                case Direction.West:
                    nextPoint -= new Point(Length, 0);
                    break;
                
                case Direction.East:
                    nextPoint += new Point(Length, 0);
                    break;
                
                case Direction.South:
                    nextPoint += new Point(0, Breadth);
                    break;
            }
            if (level.Rooms.Select(x => x.TopLeftCorner).Contains(nextPoint))
                continue;

            var dict = new Dictionary<Direction, Room> { [neighbourDirection.OppositeDirection()] = this };
            

            Neighbours[neighbourDirection] = new Room(nextPoint, dict, Length, Breadth, RoomsLeft - roomsAmount);
        }
    }
    
    public bool IsPositionInRoomBounds(Vector2 position)
    {
        var room = this;
        var topLeft = new Vector2((room.TopLeftCorner.X - 1.4f) * 50, (room.TopLeftCorner.Y - 1.4f) * 50);
        var bottomRight = new Vector2(topLeft.X + room.Length * 56, topLeft.Y + room.Breadth * 56);

        if (position.X < topLeft.X || position.Y < topLeft.Y || position.X > bottomRight.X || position.Y > bottomRight.Y) 
            return false;
        
        return true;
    }
    
    public void IsPlayerInRoomBounds(Vector2 position)
    {
        var room = this;
        var topLeft = new Vector2((room.TopLeftCorner.X - 1) * 50, (room.TopLeftCorner.Y - 1) * 50);
        var bottomRight = new Vector2(topLeft.X + (room.Length + 1) * 50, topLeft.Y + (room.Breadth + 1) * 50);

        if (position.X < topLeft.X || position.Y < topLeft.Y || position.X > bottomRight.X || position.Y > bottomRight.Y)
            PlayerIsOutsideRoom!();
    }
}