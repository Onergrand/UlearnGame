﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameModel;

public interface IGameModel
{
    Player Player { get; set; }
    Dictionary<int, IEntity> Entities { get; set; }
    event EventHandler<GameEventArgs> Updated;

    void Update();
    void ChangeGameState();
    void MovePlayer(Direction direction);
    void MakePlayerAttack(Direction direction);
    void Initialize();
}

public class GameEventArgs : EventArgs
{
    public Dictionary<int, IEntity> Entities { get; set; }    
    public Vector2 POVShift { get; set; }
    public GameState CurrentGameState { get; set; }
}