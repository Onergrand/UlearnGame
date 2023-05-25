using System;
using RoguelikeGame.GameModel;
using RoguelikeGame.GameView;

namespace RoguelikeGame;

public class GamePresenter
{
    private readonly IGameView _gameView;
    private readonly IGameModel _gameModel;

    public GamePresenter(IGameView gameView, IGameModel gameModel)
    {
        _gameView = gameView;
        _gameModel = gameModel;

        _gameModel.Updated += ModelViewUpdate;
        _gameView.CycleFinished += ViewModelUpdate;
        _gameView.PlayerAttacked += ViewModelMakePlayerAttack;
        _gameView.PlayerMoved += ViewModelMovePlayer;
        _gameView.ChangedGameState += ViewModelChangeGameState;
        _gameView.StartNewGame += ViewModelStartNewGame;
        
        _gameModel.Initialize();
    }

    public void LaunchGame()
    {
        _gameView.Run();
    }
    
    private void ViewModelUpdate(object sender, EventArgs e)
    {
        _gameModel.Update();
    }

    private void ViewModelMovePlayer(object sender, ControlsEventArgs e)
    {
        _gameModel.MovePlayer(e.Direction);
    }
    
    private void ViewModelMakePlayerAttack(object sender, ControlsEventArgs e)
    {
        _gameModel.MakePlayerAttack(e.Direction);
    }
    
    private void ViewModelChangeGameState(object sender, EventArgs e)
    {
        _gameModel.ChangeGameState();
    }
    
    private void ViewModelStartNewGame(object sender, EventArgs e)
    {
        _gameModel.StartNewGame();
    }

    private void ModelViewUpdate(object sender, GameEventArgs e)
    {
        _gameView.LoadGameCycleParameters(e.Entities, e.POVShift, e.CurrentGameState);
    }
}