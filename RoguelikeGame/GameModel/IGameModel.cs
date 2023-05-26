using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameModel;

public interface IGameModel
{
    Player Player { get; set; }
    Dictionary<int, IEntity> Entities { get; set; }
    event EventHandler<GameEventArgs> Updated;
    public event EventHandler<LevelStateArgs> UpdateLevelState;

    void Update();
    void ChangeGameState();
    void MovePlayer(Direction direction);
    void MakePlayerAttack(Direction direction);
    void StartNewGame();
    void UpdateMenuButtonsPositions(int height, int width);
    void Initialize();
}

public class GameEventArgs : EventArgs
{
    public Dictionary<int, IEntity> Entities { get; init; }    
    public Vector2 POVShift { get; init; }
    public GameState CurrentGameState { get; init; }
    
    public int PlayerId { get; init; }
}

public class LevelStateArgs : EventArgs
{
    public bool LevelFinished { get; init; }
    public GameState GameState { get; init; }
}