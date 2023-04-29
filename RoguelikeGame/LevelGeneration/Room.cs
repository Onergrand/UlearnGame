using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RoguelikeGame.LevelGeneration;

public class Room
{
    public Point TopLeftCorner { get; }
    public int _roomsLeft { get; }
    public Dictionary<Direction, Room> Neighbours;
    
    private List<Direction> _possibleDirections = new() { Direction.North, Direction.East, Direction.West, Direction.South };

    public int Length { get; }
    public int Breadth { get; }

    public Room(Point topLeftCorner, int length, int breadth, int roomsLeft)
    {
        TopLeftCorner = topLeftCorner;
        Neighbours = new Dictionary<Direction, Room>();
        Length = length;
        Breadth = breadth;
        _roomsLeft = roomsLeft;
    }
    
    public Room(Point topLeftCorner, Dictionary<Direction, Room> neighbours, int length, int breadth, int roomsLeft)
    {
        TopLeftCorner = topLeftCorner;
        Length = length;
        Breadth = breadth;
        _roomsLeft = roomsLeft;
        
        Neighbours = neighbours;
        _possibleDirections = _possibleDirections.Except(Neighbours.Keys).ToList();
    }

    public void CreateNeighbours()
    {
        var rnd = new Random();
        var roomsAmount = rnd.Next(0, _roomsLeft > _possibleDirections.Count ? _possibleDirections.Count : _roomsLeft);

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

            var dict = new Dictionary<Direction, Room>();
            dict[GetOppositeDirection(neighbourDirection)] = this;
            
            Neighbours[neighbourDirection] = new Room(nextPoint, dict, Length, Breadth, _roomsLeft - roomsAmount);
        }
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.South,
            Direction.West => Direction.East,
            Direction.East => Direction.West,
            Direction.South => Direction.North
        };
    }

}