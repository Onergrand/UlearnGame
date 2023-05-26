using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameView;

public interface IGameView
{
    event EventHandler CycleFinished;
    event EventHandler<ControlsEventArgs> PlayerMoved;
    event EventHandler<ControlsEventArgs> PlayerAttacked;
    event EventHandler ChangedGameState;
    event EventHandler StartNewGame;
    event EventHandler<ClientSizeEventArgs> ClientSizeChanged;

    void Run();
    void LoadGameCycleParameters(Dictionary<int, IEntity> entities, Vector2 POVShift, GameState currentGameState);
    void UpdateLevelState(bool levelFinished, GameState gameState);
}

public class ControlsEventArgs : EventArgs
{
    public Direction Direction { get; init; }
}

public class ClientSizeEventArgs : EventArgs
{
    public int Height { get; init; }
    public int Width { get; init; }
}