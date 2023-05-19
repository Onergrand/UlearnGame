using System;
using RoguelikeGame;
using RoguelikeGame.GameModel;
using RoguelikeGame.GameView;

public static class Program
{
    [STAThread]
    static void Main()
    {        
        var presenter = new GamePresenter(new GameCycleView(), new GameCycle());
        presenter.LaunchGame();
    }
}