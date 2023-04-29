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
    public int PlayerId { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    
    private int _currentId;
    private const int BasicSpeed = 7;

    public void MoveEnemies()
    {
        throw new NotImplementedException();
    }

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
        PlayerId = _currentId;
        _currentId++;

    }

    private void CreateLevel()
    {
        var level = new Level(6);
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
    
    public void Update()
    {
        var playerInitPos = Entities[PlayerId].Position;
        var player = (Player)Entities[PlayerId];
        
        foreach (var entity in Entities.Values.Except(new[] { Entities[PlayerId] }))
        {
            entity.Update();
            if (entity is ISolid solid)
            {
                if (RectangleCollider.IsCollided(solid.Collider, player.Collider))
                {
                    var speed = player.Speed;
                    var pos = player.Position;
                    var currentSpeed = speed - pos;
                    
                    player.Position = pos - currentSpeed;
                    player.Speed -= 1.1f * currentSpeed;
                }
            }
        }
        
        player.Update();
        var playerShift = Entities[PlayerId].Position - playerInitPos;
        
        Updated(this, new GameEventArgs { Entities = Entities, POVShift = playerShift});                  
    }

    public void MovePlayer(Direction direction)
    {
        var p = (Player)Entities[PlayerId];
        
        switch (direction)
        {
            case Direction.North:
                p.Speed += new Vector2(0, -BasicSpeed);
                break;
            
            case Direction.South:
                p.Speed += new Vector2(0, BasicSpeed);
                break;
            
            case Direction.East:
                p.Speed += new Vector2(BasicSpeed, 0);
                break;
            
            case Direction.West:
                p.Speed += new Vector2(-BasicSpeed, 0);
                break;
            
            case Direction.NorthEast:
                p.Speed += new Vector2(BasicSpeed, -BasicSpeed / 2.0f);
                break;
            
            case Direction.NorthWest:
                p.Speed += new Vector2(-BasicSpeed, -BasicSpeed / 2.0f);
                break;
            
            case Direction.SouthEast:
                p.Speed += new Vector2(BasicSpeed, BasicSpeed / 2.0f);
                break;
            
            case Direction.SouthWest:
                p.Speed += new Vector2(-BasicSpeed, BasicSpeed / 2.0f);
                break;
        }
    }
}