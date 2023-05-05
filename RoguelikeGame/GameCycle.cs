using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;
using RoguelikeGame.Creatures.Objects;
using RoguelikeGame.LevelGeneration;

namespace RoguelikeGame;

public class GameCycle : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    public Player Player { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    
    private Level _level;
    private Room _currentRoom;
    private int _currentId;
    private Vector2 _currentPOV = new(0,0);
    private const int BasicSpeed = 6;

    public void Initialize()
    {
        Entities = new Dictionary<int, IEntity>();

        CreateLevel();

        var x = new Enemy(0, new Vector2(400, 400));
        Entities.Add(_currentId, x);
        _currentId++;
        
        var player = new Player(0, 
            new Vector2(Level.InitialPos.X * Level.TileSize, Level.InitialPos.Y * Level.TileSize));
        
        Entities.Add(_currentId, player);
        Player = player;
        _currentId++;

    }

    private void CreateLevel()
    {
        var level = new Level(7);
        while (level.Rooms.Count < 4)
            level = new Level(8);

        _level = level;
        _currentRoom = _level.Rooms.First();
        _currentRoom.PlayerIsOutsideRoom += ChangeCurrentRoomIfExited;
        
        for (var i = 0; i < level.Map.GetLength(0); i++)
        {
            for (var j = 0; j < level.Map.GetLength(1); j++)
            {
                var cell = level.Map[i, j];
                if (cell == RoomObjects.Empty)
                    continue;
                
                var pos = new Vector2(i * Level.TileSize - Level.TileSize, j * Level.TileSize - Level.TileSize);
                switch (cell)
                {
                    case RoomObjects.Wall:
                        Entities.Add(_currentId, new Wall(2, pos));
                        _currentId++;
                        break;
                
                    case RoomObjects.Floor:
                    case RoomObjects.Player:
                    case RoomObjects.Exit:
                        Entities.Add(_currentId, new Floor(1, pos));
                        _currentId++;
                        break;
                }
            }
        }
    }

    private void ChangeCurrentRoomIfExited() // Проверить нужно ли каждый раз подписывать _currentRoom на ChangeCurrentRoomIfExited
    {
        var previousRoom = _currentRoom;
        foreach (var neighbour in _currentRoom.Neighbours.Values)
        {
            if (!neighbour.IsPositionInRoomBounds(Player.Position)) 
                continue;
            
            _currentRoom = neighbour;
            break;
        }
        if (previousRoom == _currentRoom)
            throw new ArgumentOutOfRangeException($"Player is outside the map bounds");

        var delta = _currentRoom.TopLeftCorner - previousRoom.TopLeftCorner;
        _currentPOV = new Vector2(delta.X * Level.TileSize, delta.Y * Level.TileSize);
        
        previousRoom.PlayerIsOutsideRoom -= ChangeCurrentRoomIfExited;
        _currentRoom.PlayerIsOutsideRoom += ChangeCurrentRoomIfExited;
    }
    
    public void Update()
    {
        var playerInitPos = Player.Position;

        _currentRoom.IsPlayerInRoomBounds(Player.Position);

        var currentEntities = Entities
            .Values
            .Except(new[] { Player })
            .Where(entity => _currentRoom.IsPositionInRoomBounds(entity.Position) && entity is ISolid);
        
        CheckCollision(currentEntities);

        var playerShift = _currentPOV;
        _currentPOV = new Vector2(0, 0);
        
        Updated!(this, new GameEventArgs { Entities = Entities, POVShift = playerShift});                  
    }

    private void CheckCollision(IEnumerable<IEntity> currentEntities)
    {
        var player = Player;
        var playerInitPos = Player.Position;
        Player.Update();
        foreach (var entity in currentEntities)
        {
            entity.Update();
            var solid = entity as ISolid;

            if (RectangleCollider.IsCollided(solid!.Collider, player.Collider))
            {
                if (playerInitPos != player.Speed)
                {
                    var intersects = Rectangle.Intersect(player.Collider.Boundary, solid.Collider.Boundary);
                    if (intersects.Width > intersects.Height)
                        player.Speed = new Vector2(player.Position.X, playerInitPos.Y);
                    else if (intersects.Width < intersects.Height)
                        player.Speed = new Vector2(playerInitPos.X, player.Position.Y);
                    Player.Update();
                }
            }
        }
    }

    public void MovePlayer(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                Player.Speed += new Vector2(0, -BasicSpeed);
                Player.Direction = Direction.North;
                break;
            
            case Direction.South:
                Player.Speed += new Vector2(0, BasicSpeed);
                Player.Direction = Direction.South;
                break;
            
            case Direction.East:
                Player.Speed += new Vector2(BasicSpeed, 0);
                Player.Direction = Direction.East;
                break;
            
            case Direction.West:
                Player.Speed += new Vector2(-BasicSpeed, 0);
                Player.Direction = Direction.West;
                break;
            
            case Direction.NorthEast:
                Player.Speed += new Vector2(BasicSpeed, -BasicSpeed / 2.0f);
                Player.Direction = Direction.NorthEast;
                break;
            
            case Direction.NorthWest:
                Player.Speed += new Vector2(-BasicSpeed, -BasicSpeed / 2.0f);
                Player.Direction = Direction.NorthWest;
                break;
            
            case Direction.SouthEast:
                Player.Speed += new Vector2(BasicSpeed, BasicSpeed / 2.0f);
                Player.Direction = Direction.SouthEast;
                break;
            
            case Direction.SouthWest:
                Player.Speed += new Vector2(-BasicSpeed, BasicSpeed / 2.0f);
                Player.Direction = Direction.SouthWest;
                break;
        }

    }
}