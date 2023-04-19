using System;
using System.Collections.Generic;
using RoguelikeGame.Creatures;

namespace RoguelikeGame;

public interface IGameView
{
    event EventHandler CycleFinished;
    event EventHandler<ControlsEventArgs> PlayerMoved; // Сообщает что игрок сдивнулся

    void Run();
    void LoadGameCycleParameters(Dictionary<int, IEntity> entities);
}

public class ControlsEventArgs : EventArgs
{
    public Direction Direction { get; set; }
}