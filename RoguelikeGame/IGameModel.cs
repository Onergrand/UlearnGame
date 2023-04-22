using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame;

public interface IGameModel
{
    int PlayerId { get; set; }
    Dictionary<int, IEntity> Entities { get; set; }
    event EventHandler<GameEventArgs> Updated;

    void Update();
    void MovePlayer(Direction direction);
    void MoveEnemies();
    void Initialize();
}

public class GameEventArgs : EventArgs
{
    public Dictionary<int, IEntity> Entities { get; set; }    
    public Vector2 POVShift { get; set; }
}