using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame;

public class GameCycle : IGameModel
{
    private const int BasicSpeed = 2;
    
    public event EventHandler<GameEventArgs> Updated;
    
    public int PlayerId { get; set; }

    public Dictionary<int, IEntity> Entities { get; set; }


    public void MoveEnemies()
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
        Entities = new Dictionary<int, IEntity>();
        var player = new Player(0, new Vector2(200, 200), new Vector2(0,0));
        
        Entities.Add(player.Id, player);
        PlayerId = player.Id;
    }
    
    public void Update()
    {
        foreach (var o in Entities.Values)
            o.Update();
        
        
        Updated(this, new GameEventArgs { Entities = Entities });                  
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
                p.Speed += new Vector2(BasicSpeed, -BasicSpeed / 2);
                break;
            
            case Direction.NorthWest:
                p.Speed += new Vector2(-BasicSpeed, -BasicSpeed / 2);
                break;
            
            case Direction.SouthEast:
                p.Speed += new Vector2(BasicSpeed, BasicSpeed / 2);
                break;
            
            case Direction.SouthWest:
                p.Speed += new Vector2(-BasicSpeed, BasicSpeed / 2);
                break;
        }
    }
}