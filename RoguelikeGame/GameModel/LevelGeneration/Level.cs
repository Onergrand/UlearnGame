using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameModel.LevelGeneration;

public class Level
{
    public int LevelNumber;
    public RoomObjects[,] Map { get; }
    public List<Room> Rooms { get; private set; } = new();
    
    public const int TileSize = 50;
    public static readonly Point InitialPos = new(49, 49);

    private readonly int _minimumRoomsAmountToCreate;

    public Level(int minimumRoomsAmount)
    {
        _minimumRoomsAmountToCreate = minimumRoomsAmount;
        Map = new RoomObjects[100, 100];
        CreateLevel(InitialPos);
    }
    
    private void CreateLevel(Point startPos)
    {
        var roomLength = 11;
        var roomBreadth = 9;

        CreateRooms(startPos, roomLength, roomBreadth);

        Rooms = Rooms.Distinct().ToList();

        FillRoomWithWallsAndFloor(Rooms.First().TopLeftCorner, roomLength, roomBreadth);
        foreach (var room in Rooms.Skip(1))
        {
            FillRoomWithWallsAndFloor(room.TopLeftCorner, roomLength, roomBreadth);
            FillRoomWithMonsters(room.TopLeftCorner, roomLength, roomBreadth);
        }

        PlaceExits();
        Map[50, 50] = RoomObjects.Player;
    }
    
    private void FillRoomWithMonsters(Point startPos, int length, int breadth)
    {
        var rnd = new Random();
        var monstersAmount = rnd.Next(0, 4);

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

    private void CreateRooms(Point startPos, int length, int breadth)
    {
        var currentRoom = new Room(startPos, length, breadth, _minimumRoomsAmountToCreate);
        currentRoom.CreateNeighbours(this);
        Rooms.Add(currentRoom);
        foreach (var room in currentRoom.Neighbours.Values)
        {
            Rooms.Add(room);
            room.CreateNeighbours(this);

            foreach (var nextRoom in room.Neighbours.Values)
                Rooms.Add(nextRoom);

            Rooms = Rooms.Distinct().ToList();
        }
        
        while (Rooms.Count < _minimumRoomsAmountToCreate )
        {
            var rooms = Rooms.ToArray();
            foreach (var room in rooms)
            {
                room.CreateNeighbours(this);

                foreach (var nextRoom in room.Neighbours.Values)
                    Rooms.Add(nextRoom);
                
                Rooms = Rooms.Distinct().ToList();
                
                if (Rooms.Count == _minimumRoomsAmountToCreate)
                    break;
            }
            Rooms = Rooms.Distinct().ToList();
        }
    }

    private void PlaceExits()
    {
        foreach (var room in Rooms)
        {
            foreach (var neighbourDir in room.Neighbours.Keys)
            {
                var exit = room.TopLeftCorner;
                var secondExit = exit;
                switch (neighbourDir)
                {
                    case Direction.North:
                        exit += new Point(room.Length / 2, 0);
                        secondExit += new Point(room.Length / 2 + 1, 0);
                        break;
                
                    case Direction.West:
                        exit += new Point(0, room.Breadth / 2);
                        secondExit += new Point(0, room.Breadth / 2 + 1);
                        break;
                
                    case Direction.East:
                        exit -= new Point(0, room.Breadth / 2);
                        secondExit -= new Point(0, room.Breadth / 2 + 1);
                        break;
                
                    case Direction.South:
                        exit -= new Point(room.Length / 2, 0);
                        secondExit -= new Point(0, room.Breadth / 2 + 1);
                        break;
                }

                
                Map[exit.X, exit.Y] = RoomObjects.Exit;
                Map[secondExit.X, secondExit.Y] = RoomObjects.Exit;
                
                
                if (GetNeighbors(exit).Any(point => Map[point.X, point.Y] == RoomObjects.Empty))
                    Map[exit.X, exit.Y] = RoomObjects.Wall;
                
                if (GetNeighbors(secondExit).Any(point => Map[point.X, point.Y] == RoomObjects.Empty))
                    Map[secondExit.X, secondExit.Y] = RoomObjects.Wall;
            }
        }
    }

    private void FillRoomWithWallsAndFloor(Point startPos, int length, int breadth)
    {
        for (var i = startPos.X; i < startPos.X + length + 1; i++)
        {
            for (var j = startPos.Y; j < startPos.Y + breadth + 1; j++)
            {
                if (Map[i, j] == RoomObjects.Exit)
                {
                    if (GetNeighbors(new Point(i, j)).Any(point => Map[point.X, point.Y] == RoomObjects.Empty))
                        Map[i, j] = RoomObjects.Wall;

                    continue;
                }

                if (IsPointOnMapEdge(startPos, i, j, length, breadth))
                    Map[i, j] = RoomObjects.Wall;
                else
                    Map[i, j] = RoomObjects.Floor;
            }
        }
    }
    
    private static IEnumerable<Point> GetNeighbors(Point point)
    {
        return new[]
        {
            new Point(point.X + 1, point.Y),
            new Point(point.X - 1, point.Y),
            new Point(point.X, point.Y + 1),
            new Point(point.X, point.Y - 1)
        };
    }

    private Point CreateNewPoint(Point startPos, int length, int breadth)
    {
        var rnd = new Random();
        
        return new Point(
            rnd.Next(startPos.X + 1, startPos.X + length - 1),
            rnd.Next(startPos.Y + 1, startPos.Y + breadth - 1));
    }
    
    private bool IsPointOnMapEdge(Point startPos, int i, int j, int length, int breadth)
    {
        var x = startPos.X;
        var y = startPos.Y;
        return i == x || i == x + length || j == y || j == y + breadth;
    }
}