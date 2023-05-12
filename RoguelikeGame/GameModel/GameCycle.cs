using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures.Objects;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.LevelGeneration;

namespace RoguelikeGame.GameModel;

public partial class GameCycle : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    public Player Player { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    
    private Level _level;
    private Room _currentRoom;
    private int _currentId;
    private Vector2 _currentPov = new(0,0);
    private const int BasicSpeed = 4;

    public void Initialize()
    {
        Entities = new Dictionary<int, IEntity>();

        CreateLevel();

        var player = new Player(0, 
            new Vector2(Level.InitialPos.X * Level.TileSize, Level.InitialPos.Y * Level.TileSize));
        
        Entities.Add(_currentId, player);
        Player = player;
        _currentId++;

    }

    private void CreateLevel()
    {
        var level = new Level(7);

        _level = level;
        var x = level.Rooms.Count;
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
                        Entities.Add(_currentId, EnemyType.CreateNewEnemy(pos));
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

    public void Update()
    {
        _currentRoom.IsPlayerInRoomBounds(Player.Position);

        var currentEntities = Entities
            .Values
            .Where(entity => _currentRoom.IsPositionInRoomBounds(entity.Position) && entity is ISolid);
        
        var currentEntitiesWithKeys = Entities
            .Where(entity => 
                _currentRoom.IsPositionInRoomBounds(entity.Value.Position) && 
                entity.Value is ISolid /*and not RoguelikeGame.Entities.Creatures.Player*/)
            .ToDictionary(x => x.Key, y => y.Value);

        CheckBulletCollision(currentEntitiesWithKeys);
        CheckCollision(currentEntities);

        var playerShift = _currentPov;
        _currentPov = new Vector2(0, 0);
        
        Updated!(this, new GameEventArgs { Entities = Entities, POVShift = playerShift});                  
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
        }
    }

    public void MakePlayerAttack(Direction direction)
    {
        if ((DateTime.Now - Player.LastShotTime).Milliseconds < 500)
            return;
        
        Player.LastShotTime = DateTime.Now;
        
        var attackDirection = direction;
        var playerPosition = Player.Position;

        var bulletStartPosition = attackDirection switch
        {
            Direction.North => new Vector2(playerPosition.X + 25, playerPosition.Y - 20),
            Direction.East => new Vector2(playerPosition.X + 50, playerPosition.Y + 25),
            Direction.West => new Vector2(playerPosition.X - 20, playerPosition.Y + 25),
            Direction.South => new Vector2(playerPosition.X + 25, playerPosition.Y + 50)
        };

        var bullet = new Bullet(4, bulletStartPosition, attackDirection.ConvertToVector() * 6, Player.Damage);
        Entities.Add(Entities.Keys.Max() + 1, bullet);
        _currentId++;
    }
}