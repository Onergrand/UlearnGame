using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace RoguelikeGame.LevelGeneration;

public class Level
{
    public int LevelNumber;
    public RoomObjects[,] Map { get; }
    
    public const int TileSize = 50;
    public static readonly Point InitialPos = new(49, 49);
    
    private List<Room> _rooms = new();
    private readonly int _roomsAmountToCreate;

    public Level(int roomsAmount)
    {
        _roomsAmountToCreate = roomsAmount;
        Map = new RoomObjects[100, 100];
        CreateLevel(InitialPos);
    }
    
    private void CreateLevel(Point startPos)
    {
        var rnd = new Random();
        var roomLength = rnd.Next(11, 12); // rnd.Next(9, 15);
        var roomBreadth = rnd.Next(9, 10); // rnd.Next(7, 10);

        CreateRooms(startPos, roomLength, roomBreadth);

        _rooms = _rooms.Distinct().ToList();
        
        foreach (var room in _rooms)
            FillRoomWithWallsAndFloor(room.TopLeftCorner, roomLength, roomBreadth);

        PlaceExits();
        Map[50, 50] = RoomObjects.Player;
    }

    private void CreateRooms(Point startPos, int length, int breadth)
    {
        var currentRoom = new Room(startPos, length, breadth, _roomsAmountToCreate);
        currentRoom.CreateNeighbours();
        _rooms.Add(currentRoom);

        foreach (var room in currentRoom.Neighbours.Values)
        {
            _rooms.Add(room);
            room.CreateNeighbours();
            
            foreach (var nextRoom in room.Neighbours.Values)
                _rooms.Add(nextRoom);
            
            var roomsLeft = room.Neighbours.Values.First()._roomsLeft;
            if (roomsLeft == 0) 
                break;
        }
    }

    private void PlaceExits()
    {
        foreach (var room in _rooms)
        {
            foreach (var neighbourDir in room.Neighbours.Keys)
            {
                var exit = room.TopLeftCorner;
                
                switch (neighbourDir)
                {
                    case Direction.North:
                        exit += new Point(room.Length / 2, 0);
                        break;
                
                    case Direction.West:
                        exit += new Point(0, room.Breadth / 2);
                        break;
                
                    case Direction.East:
                        exit -= new Point(0, room.Breadth / 2);
                        break;
                
                    case Direction.South:
                        exit -= new Point(room.Length / 2, 0);
                        break;
                }

                Map[exit.X, exit.Y] = RoomObjects.Exit;
            }
        }
    }

    private void FillRoomWithWallsAndFloor(Point startPos, int length, int breadth)
    {
        /*var topLeftCornerPos = direction switch
        {
            Direction.North => new Point(startPos.X - length, startPos.Y),
            Direction.West => new Point(startPos.X, startPos.Y - breadth),
            Direction.East => startPos,
            Direction.South => startPos
        };
        if (IsInitialPoint(startPos)) 
            topLeftCornerPos = InitialPos; */

        for (var i = startPos.X; i < startPos.X + length + 1; i++)
        {
            for (var j = startPos.Y; j < startPos.Y + breadth + 1; j++)
            {
                if (Map[i, j] == RoomObjects.Exit)
                    continue;

                if (IsPointOnMapEdge(startPos, i, j, length, breadth))
                    Map[i, j] = RoomObjects.Wall;
                else
                    Map[i, j] = RoomObjects.Floor;
            }
        }
    }

    private void FillRoomWithMonsters(Point startPos, int length, int breadth)
    {
        var rnd = new Random();
        var temp = (length - 2) * (breadth - 2);
        var monstersAmount = temp > 60 ? rnd.Next(5, temp / 10) : rnd.Next(2, temp / 7);

        var monstersPositions = new HashSet<Point>();

        for (var i = 0; i < monstersAmount; i++)
        {
            var pos = CreateNewPoint(startPos, length, breadth);
            while (monstersPositions.Contains(pos))
                pos = CreateNewPoint(startPos, length, breadth);

            monstersPositions.Add(pos);
        }
        
        foreach (var point in monstersPositions)
            Map[point.X, point.Y] = RoomObjects.Monster;
    }

    private Point CreateNewPoint(Point startPos, int length, int breadth)
    {
        var rnd = new Random();
        
        return new Point(
            rnd.Next(startPos.X + 1, startPos.X + length - 1),
            rnd.Next(startPos.Y + 1, startPos.Y + breadth - 1));
    }

    public static bool IsInitialPoint(Point point) => InitialPos == point;
    
    private bool IsPointOnMapEdge(Point startPos, int i, int j, int length, int breadth)
    {
        var x = startPos.X;
        var y = startPos.Y;
        return i == x || i == x + length || j == y || j == y + breadth;
    }
}