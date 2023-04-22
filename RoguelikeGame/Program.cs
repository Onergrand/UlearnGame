using System;
using RoguelikeGame;

public static class Program
{
    [STAThread]
    static void Main()
    {        
        var presenter = new GamePresenter(new GameCycleView(), new GameCycle());
        presenter.LaunchGame();
    }
}