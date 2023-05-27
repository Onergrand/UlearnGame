using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures.Objects;
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
    
    private Vector2 _lastKnownPlayerSpeed;
    private Vector2 _currentPov = new(0,0);
    
    private const int BasicSpeed = 3;

    public void Initialize()
    {
        switch (_currentGameState)
        {
            case GameState.Game:
                InitializeGame(1);
                break;
            
            case GameState.Menu:
                InitializeMenu();
                break;
        }
    }

    private void InitializeGame(int currentLevelNumber)
    {
        _currentLevelNumber = currentLevelNumber;
        Entities = new Dictionary<int, IEntity>();
        CreateLevel(currentLevelNumber);

        var player = new Player(0, 
            new Vector2(Level.InitialPos.X * Level.TileSize, Level.InitialPos.Y * Level.TileSize), _currentId);
        
        Entities.Add(_currentId, player);
        
        Player = player;
        _currentId++;
        
        UpdateGame();
        UpdateLevelState(this, new LevelStateArgs{ LevelFinished = false, GameState = _currentGameState});
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
    
    public void UpdateMenuButtonsPositions(int height, int width)
    {
        var x = 30;
        var y = height / 1.4f;

        var startGameButton = _buttons[0] as Button;
        startGameButton!.MoveCollider(new Vector2(x, y));

        var exitButton = _buttons[1] as Button;
        exitButton!.MoveCollider( new Vector2(x, y + 80));
    }

    public void StartNewGame()
    {
        _currentGameState = GameState.Game;
        Initialize();
    }

    public void ChangeGameState() => _currentGameState = _currentGameState.GetOppositeState();

    private void CreateLevel(int currentLevelNumber)
    {
        var roomsAmountToAdd = currentLevelNumber / 5;
        var level = new Level(3 + roomsAmountToAdd);
        
        _currentLevel = level;
        _remainingMonstersAmount = level.MonstersCreated;
        _currentRoom = _currentLevel.Rooms.First();
        _currentRoom.PlayerIsOutsideRoom += ChangeCurrentRoomIfExited;
        
        for (var i = 0; i < level.Map.GetLength(0); i++)
        {
            for (var j = 0; j < level.Map.GetLength(1); j++)
            {
                var cell = level.Map[i, j];
                if (cell == RoomObjects.Empty)
                    continue;
                
                var pos = new Vector2(i * Level.TileSize - Level.TileSize, j * Level.TileSize - Level.TileSize);
                FillCell(cell, pos);
            }
        }
    }

    private void FillCell(RoomObjects cell, Vector2 pos)
    {
        switch (cell)
        {
            case RoomObjects.Wall:
                Entities.Add(_currentId, new Wall(2, pos, _currentId));
                _currentId++;
                break;

            case RoomObjects.Monster:
                Entities.Add(_currentId, new Floor(1, pos, _currentId));
                _currentId++;
                Entities.Add(_currentId, EnemyType.CreateNewEnemy(pos, _currentId));
                _currentId++;
                break;

            case RoomObjects.Floor:
            case RoomObjects.Player:
            case RoomObjects.Exit:
                Entities.Add(_currentId, new Floor(1, pos, _currentId));
                _currentId++;
                break;
        }
    }

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