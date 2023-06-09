using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameModel.LevelGeneration;

public class Level
{
    public RoomObject[,] Map { get; }
    public List<Room> Rooms { get; private set; } = new();
    
    public const int TileSize = 50;
    public static readonly Point InitialPos = new(49, 49);
    public int MonstersCreated;
    public int MonsterAmountToCreate;
    
    private readonly int _roomsAmountToCreate;
    private readonly Dictionary<int, int> _roomsByMonsterCount = new();
    
    private const int RoomLength = 11;
    private const int RoomWidth = 9;

    public Level(int roomsAmount)
    {
        _roomsAmountToCreate = roomsAmount;
        Map = new RoomObject[100, 100];
        
        SetMonstersAmount();
        CreateLevel(InitialPos);
    }

    private void SetMonstersAmount()
    {
        var roomsWithMonstersCount = _roomsAmountToCreate - 1;
        
        _roomsByMonsterCount[3] = (int)Math.Round(roomsWithMonstersCount * 0.15);
        _roomsByMonsterCount[2] = (int)Math.Round(roomsWithMonstersCount * 0.3);
        _roomsByMonsterCount[1] = (int)Math.Round(roomsWithMonstersCount * 0.35);
        _roomsByMonsterCount[0] = (int)Math.Round(roomsWithMonstersCount * 0.15);

        foreach (var (monstersAmount, roomsCount) in _roomsByMonsterCount)
        {
            MonsterAmountToCreate += monstersAmount * roomsCount;
            roomsWithMonstersCount -= roomsCount;
        }

        if (roomsWithMonstersCount == 0) return;
        
        _roomsByMonsterCount[1] += roomsWithMonstersCount;
        MonsterAmountToCreate += roomsWithMonstersCount;

    }
    
    private void CreateLevel(Point startPos)
    {
        CreateRooms(startPos);

        Rooms = Rooms.Distinct().ToList();

        FillRoomWithWallsAndFloor(Rooms.First().TopLeftCorner);
        foreach (var room in Rooms.Skip(1))
        {
            FillRoomWithWallsAndFloor(room.TopLeftCorner);
            FillRoomWithMonsters(room.TopLeftCorner);
        }

        PlaceExits();
        Map[50, 50] = RoomObject.Player;
    }
    
    private void FillRoomWithMonsters(Point startPos)
    {
        var rnd = new Random();
        var monstersAmount = rnd.Next(4);
        while (_roomsByMonsterCount[monstersAmount] == 0)
            monstersAmount = rnd.Next(4);
        
        MonstersCreated += monstersAmount;
        _roomsByMonsterCount[monstersAmount]--;
        
        var monstersPositions = new HashSet<Point>();

        for (var i = 0; i < monstersAmount; i++)
        {
            var pos = CreateNewRandomPoint(startPos, RoomLength, RoomWidth);
            while (monstersPositions.Contains(pos))
                pos = CreateNewRandomPoint(startPos, RoomLength, RoomWidth);

            monstersPositions.Add(pos);
        }
        
        foreach (var point in monstersPositions)
            Map[point.X, point.Y] = RoomObject.Monster;
    }

    private void CreateRooms(Point startPos)
    {
        var currentRoom = new Room(startPos, RoomLength, RoomWidth);
        Rooms.Add(currentRoom);
        
        for (var i = 0; i < _roomsAmountToCreate - 1; i++)
        {
            currentRoom = Rooms.Last();
            var direction = GetRandomMainDirection(currentRoom.Neighbours.Keys);
            while (direction == Direction.Null)
            {
                currentRoom = GetRandomExistingRoom();
                direction = GetRandomMainDirection(currentRoom.Neighbours.Keys);
            }

            var created = currentRoom.TryCreateNeighbour(direction);

            while (!created)
            {
                currentRoom = GetRandomExistingRoom();
                direction = GetRandomMainDirection(currentRoom.Neighbours.Keys);
                    
                if (direction == Direction.Null) continue;
                    
                created = currentRoom.TryCreateNeighbour(direction);
            }

            foreach (var room in currentRoom.Neighbours.Values.Except(Rooms)) Rooms.Add(room);
        }
        
        UpdateNeighbours();
    }

    private void UpdateNeighbours()
    {
        foreach (var room in Rooms)
        foreach (var (neighbourDirection, neighbour) in room.Neighbours)
        {
            var oppositeDirection = neighbourDirection.OppositeDirection();
            if (!neighbour.Neighbours.ContainsKey(oppositeDirection))
                neighbour.Neighbours[oppositeDirection] = room;
        }
    }

    private Room GetRandomExistingRoom()
    {
        var rnd = new Random();
        return Rooms[rnd.Next(Rooms.Count)];
    }
    
    private Direction GetRandomMainDirection(IEnumerable<Direction> usedDirections)
    {
        var mainDirections = new List<Direction> { Direction.North, Direction.East, Direction.South, Direction.West };

        var directions = usedDirections.ToArray();
        if (directions.Length != 0)
            foreach (var direction in directions)
                mainDirections.Remove(direction);

        if (mainDirections.Count == 0)
            return Direction.Null;
        
        var rnd = new Random();
        if (mainDirections.Count > 1)
            mainDirections.RemoveAt(rnd.Next(mainDirections.Count));
        
        return mainDirections[rnd.Next(mainDirections.Count)];
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
                        exit += new Point(0, room.Width / 2);
                        secondExit += new Point(0, room.Width / 2 + 1);
                        break;
                
                    case Direction.East:
                        exit -= new Point(0, room.Width / 2);
                        secondExit -= new Point(0, room.Width / 2 + 1);
                        break;
                
                    case Direction.South:
                        exit -= new Point(room.Length / 2, 0);
                        secondExit -= new Point(0, room.Width / 2 + 1);
                        break;
                }

                
                Map[exit.X, exit.Y] = RoomObject.Exit;
                Map[secondExit.X, secondExit.Y] = RoomObject.Exit;
                
                
                if (GetNeighbors(exit).Any(point => Map[point.X, point.Y] == RoomObject.Empty))
                    Map[exit.X, exit.Y] = RoomObject.Wall;
                
                if (GetNeighbors(secondExit).Any(point => Map[point.X, point.Y] == RoomObject.Empty))
                    Map[secondExit.X, secondExit.Y] = RoomObject.Wall;
            }
        }
    }

    private void FillRoomWithWallsAndFloor(Point startPos)
    {
        for (var i = startPos.X; i < startPos.X + RoomLength + 1; i++)
        {
            for (var j = startPos.Y; j < startPos.Y + RoomWidth + 1; j++)
            {
                if (Map[i, j] == RoomObject.Exit)
                {
                    if (GetNeighbors(new Point(i, j)).Any(point => Map[point.X, point.Y] == RoomObject.Empty))
                        Map[i, j] = RoomObject.Wall;

                    continue;
                }

                if (IsPointOnMapEdge(startPos, i, j, RoomLength, RoomWidth))
                    Map[i, j] = RoomObject.Wall;
                else
                    Map[i, j] = RoomObject.Floor;
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

    private Point CreateNewRandomPoint(Point startPos, int length, int breadth)
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