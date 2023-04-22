using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame;

public class GameCycle : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    public int PlayerId { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    
    private int _currentId;
    private const int BasicSpeed = 5;

    public void MoveEnemies()
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
        Entities = new Dictionary<int, IEntity>();
        var player = new Player(0, new Vector2(512-50, 384));
        Entities.Add(_currentId, player);
        PlayerId = player.ImageId;
        _currentId++;

        var x = new Enemy(0, new Vector2(400, 400));
        Entities.Add(_currentId, x);
        _currentId++;

    }
    
    public void Update()
    {
        var playerInitPos = Entities[PlayerId].Position;
        foreach (var o in Entities.Values)
            o.Update();
        
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