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
        _gameModel.UpdateLevelState += ModelViewUpdateLevelState;
        _gameView.CycleFinished += ViewModelUpdate;
        _gameView.PlayerAttacked += ViewModelMakePlayerAttack;
        _gameView.PlayerMoved += ViewModelMovePlayer;
        _gameView.ChangedGameState += ViewModelChangeGameState;
        _gameView.StartNewGame += ViewModelStartNewGame;
        _gameView.ClientSizeChanged += ViewModelUpdateClientParameters;
        
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
    
    private void ViewModelUpdateClientParameters(object sender, ClientSizeEventArgs e)
    {
        _gameModel.UpdateMenuButtonsPositions(e.Height, e.Width);
    }
    
    private void ModelViewUpdateLevelState(object sender, LevelStateArgs e)
    {
        _gameView.UpdateLevelState(e.LevelFinished, e.GameState);
    }

    private void ModelViewUpdate(object sender, GameEventArgs e)
    {
        _gameView.LoadGameCycleParameters(e.Entities, e.POVShift, e.CurrentGameState, e.PlayerId);
    }
}