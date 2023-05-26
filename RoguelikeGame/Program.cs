using RoguelikeGame.GameModel;
using RoguelikeGame.GameView;

namespace RoguelikeGame;

public static class Program
{
    public static void Main()
    {        
        var presenter = new GamePresenter(new GameCycleView(), new GameCycle());
        presenter.LaunchGame();
    }
}