using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Objects;

namespace RoguelikeGame.GameModel;

public partial class GameCycle
{
    private void UpdateMenu()
    {
        var mouseState = Mouse.GetState();
        foreach (var button in _buttons.Values.Cast<Button>())
            button.Update(mouseState);

        
        Updated!(this, new GameEventArgs
        {
            Entities = _buttons,
            POVShift = _cameraDeltaPosition,
            CurrentGameState = _currentGameState,
            PlayerId = Player?.Id ?? 0,
            CurrentLevelNumber = _currentLevelNumber
        });
        
        _cameraDeltaPosition = Vector2.Zero;
    }
    
    public void UpdateMenuButtonsPositions(int height)
    {
        var x = 30;
        var y = height / 1.4f;

        var startGameButton = _buttons[0] as Button;
        startGameButton!.MoveCollider(new Vector2(x, y));

        var exitButton = _buttons[1] as Button;
        exitButton!.MoveCollider( new Vector2(x, y + 80));
    }
    private void InitializeMenu()
    {
        var startGameButtonPosition = new Vector2(30, 515);
        var startGameButton = new Button(7, startGameButtonPosition, 300, 40, "Start new game", 0, Color.White);
        var exitButton = new Button(8, startGameButtonPosition + new Vector2(0, 80), 80, 40, "Exit", 1, Color.White);
        
        _buttons = new Dictionary<int, IEntity>
        {
            {0, startGameButton},
            {1, exitButton}
        };
        
        UpdateMenu();
    }
}