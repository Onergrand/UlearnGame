using System;

namespace RoguelikeGame.GameModel.Helpers;

public enum GameState : byte
{
    Game, 
    Menu
}

public static class GameStateExtensions
{
    public static GameState GetOppositeState(this GameState gameState)
    {
        return gameState switch
        {
            GameState.Game => GameState.Menu,
            GameState.Menu => GameState.Game,
            _ => throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null)
        };
    }
}