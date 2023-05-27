using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.Helpers;
using RoguelikeGame.GameModel.LevelGeneration;

namespace RoguelikeGame.GameModel;

public partial class GameCycle : IGameModel
{
    public event EventHandler<GameEventArgs> Updated;
    public Player Player { get; set; }
    public Dictionary<int, IEntity> Entities { get; set; }
    private Dictionary<int, IEntity> _buttons;
    
    private GameState _currentGameState = GameState.Menu;
    
    private Level _currentLevel;
    private Room _currentRoom;
    private int _currentLevelNumber;

    private int _currentId;

    private const int BasicSpeed = 3;

    public void Initialize()
    {
        switch (_currentGameState)
        {
            case GameState.Game:
                InitializeGame(1, 0);
                break;
            
            case GameState.Menu:
                InitializeMenu();
                break;
        }
    }

    public void Update()
    {
        switch (_currentGameState)
        {
            case GameState.Game:
                UpdateGame();
                if (_currentGameState == GameState.Menu)
                    UpdateMenu();
                break;
            
            case GameState.Menu:
                UpdateMenu();
                break;
        }
    }
    
    public void StartNewGame()
    {
        _currentGameState = GameState.Game;
        Initialize();
    }

    public void ChangeGameState() => _currentGameState = _currentGameState.GetOppositeState();

    public void MovePlayer(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                Player.Speed += new Vector2(0, -BasicSpeed);
                break;
            
            case Direction.South:
                Player.Speed += new Vector2(0, BasicSpeed);
                break;
            
            case Direction.East:
                Player.Speed += new Vector2(BasicSpeed, 0);
                break;
            
            case Direction.West:
                Player.Speed += new Vector2(-BasicSpeed, 0);
                break;
        }
    }

    public void MakePlayerAttack(Direction direction)
    {
        if ((DateTime.Now - Player.LastShotTime).Milliseconds < 500)
            return;
        
        Player.LastShotTime = DateTime.Now;
        
        var attackDirection = direction;
        var playerPosition = Player.Position;

        Vector2 bulletStartPosition;
        Vector2 deltaSpeed;
        switch (attackDirection)
        {
            case Direction.North:
                bulletStartPosition = new Vector2(playerPosition.X + 25, playerPosition.Y - 20);
                deltaSpeed = new Vector2(_lastKnownPlayerSpeed.X, 0);
                break;
            case Direction.East:
                bulletStartPosition = new Vector2(playerPosition.X + 50, playerPosition.Y + 25);
                deltaSpeed = new Vector2(0,  _lastKnownPlayerSpeed.Y);
                break;
            case Direction.West:
                bulletStartPosition = new Vector2(playerPosition.X - 20, playerPosition.Y + 25);
                deltaSpeed = new Vector2(0, _lastKnownPlayerSpeed.Y);
                break;
            case Direction.South:
                bulletStartPosition = new Vector2(playerPosition.X + 25, playerPosition.Y + 50);
                deltaSpeed = new Vector2(_lastKnownPlayerSpeed.X, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackDirection));
        }

        var id = Entities.Keys.Max() + 1;
        var bullet = new Bullet(4, bulletStartPosition, deltaSpeed / 2 + attackDirection.ConvertToVector() * 6, Player.Damage, id);
        Entities.Add(id, bullet);
        _currentId++;
    }
}