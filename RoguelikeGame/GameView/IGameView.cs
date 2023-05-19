using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;
using RoguelikeGame.Entities;
using RoguelikeGame.GameModel;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameView;

public interface IGameView
{
    event EventHandler CycleFinished;
    event EventHandler<ControlsEventArgs> PlayerMoved;
    event EventHandler<ControlsEventArgs> PlayerAttacked;
    event EventHandler ChangedGameState;

    void Run();
    void LoadGameCycleParameters(Dictionary<int, IEntity> entities, Vector2 POVShift, GameState currentGameState);
}

public class ControlsEventArgs : EventArgs
{
    public Direction Direction { get; set; }
}