using System;

namespace RoguelikeGame;

public class GamePresenter
{
    private IGameView _gameView;
    private IGameModel _gameModel;

    public GamePresenter(IGameView gameView, IGameModel gameModel)
    {
        _gameView = gameView;
        _gameModel = gameModel;

        _gameModel.Updated += ModelViewUpdate;
        _gameView.CycleFinished += ViewModelUpdate;
        _gameView.PlayerMoved += ViewModelMovePlayer;
        _gameView.PlayerAttacked += ViewModelMakePlayerAttack;
        
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
    
    private void ViewModelMakePlayerAttack(object sender, EventArgs e)
    {
        _gameModel.MakePlayerAttack();
    }

    private void ModelViewUpdate(object sender, GameEventArgs e)
    {
        _gameView.LoadGameCycleParameters(e.Entities, e.POVShift);
    }
}