using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;
using RoguelikeGame.Creatures.Objects;
using RoguelikeGame.Entities.Creatures;
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
                    
                    case RoomObjects.Monster:
                        Entities.Add(_currentId, new Floor(1, pos));
                        _currentId++;
                        Entities.Add(_currentId, new Enemy(3, pos));
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

    private void ChangeCurrentRoomIfExited()
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
        _currentRoom.IsPlayerInRoomBounds(Player.Position);

        var currentEntities = Entities
            .Values
            .Except(new[] { Player })
            .Where(entity => _currentRoom.IsPositionInRoomBounds(entity.Position) && entity is ISolid);
        
        var currentEntitiesWithKeys = Entities
            .Where(entity => 
                _currentRoom.IsPositionInRoomBounds(entity.Value.Position) && 
                entity.Value is ISolid and not RoguelikeGame.Entities.Creatures.Player)
            .ToDictionary(x => x.Key, y => y.Value);

        CheckBulletCollision(currentEntitiesWithKeys);
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
            if (entity is Enemy monster)
                monster.MoveToPlayer(Player.Position);
                    
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

    private void CheckBulletCollision(Dictionary<int,IEntity> currentEntities)
    {
        var bullets = currentEntities
            .Where(x => x.Value is Bullet)
            .ToDictionary(x => x.Key, y => y.Value);
        
        foreach (var bulletKeyValuePair in bullets)
        {
            var id = bulletKeyValuePair.Key;
            var bullet = bulletKeyValuePair.Value as Bullet;
            foreach (var entity in currentEntities.Select(x => x.Value).Except(bullets.Values).OfType<ISolid>())
            {
                if (RectangleCollider.IsCollided(entity.Collider, bullet!.Collider))
                    Entities.Remove(id);
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
                break;
            
            case Direction.NorthWest:
                Player.Speed += new Vector2(-BasicSpeed, -BasicSpeed / 2.0f);
                break;
            
            case Direction.SouthEast:
                Player.Speed += new Vector2(BasicSpeed, BasicSpeed / 2.0f);
                break;
            
            case Direction.SouthWest:
                Player.Speed += new Vector2(-BasicSpeed, BasicSpeed / 2.0f);
                break;
        }
    }

    public void MakePlayerAttack()
    {
        var attackDirection = Player.Direction;
        var playerPosition = Player.Position;

        var bulletStartPosition = attackDirection switch
        {
            Direction.North => new Vector2(playerPosition.X + 25, playerPosition.Y - 20),
            Direction.East => new Vector2(playerPosition.X + 50, playerPosition.Y + 25),
            Direction.West => new Vector2(playerPosition.X - 20, playerPosition.Y + 25),
            Direction.South => new Vector2(playerPosition.X + 25, playerPosition.Y + 50)
        };

        var bullet = new Bullet(4, bulletStartPosition, attackDirection.ConvertToVector() * 2);
        Entities.Add(_currentId, bullet);
        _currentId++;
    }
}