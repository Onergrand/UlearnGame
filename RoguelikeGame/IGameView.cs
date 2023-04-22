using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame;

public interface IGameView
{
    event EventHandler CycleFinished;
    event EventHandler<ControlsEventArgs> PlayerMoved; // Сообщает что игрок сдивнулся

    void Run();
    void LoadGameCycleParameters(Dictionary<int, IEntity> entities, Vector2 POVShift);
}

public class ControlsEventArgs : EventArgs
{
    public Direction Direction { get; set; }
}