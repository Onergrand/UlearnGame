using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.Helpers;
using RoguelikeGame.GameModel.LevelGeneration;

namespace RoguelikeGame.GameView;


public partial class GameCycleView : Game, IGameView
{
    public event EventHandler CycleFinished;
    public event EventHandler<ControlsEventArgs> PlayerMoved;
    public event EventHandler<ControlsEventArgs> PlayerAttacked;
    public event EventHandler ChangedGameState;
    public event EventHandler StartNewGame;
    public event EventHandler<ClientSizeEventArgs> ClientSizeChanged;

    private Dictionary<int, IEntity> _entities = new();
    private GameState _currentGameState;
    private readonly Dictionary<int, Texture2D> _textures = new();
    private readonly Dictionary<int, Animation> _animations = new(); 
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private DateTime _lastTimeExitButtonPressed = DateTime.Now;
    private SpriteFont _buttonFont;

    private float _backgroundScaling;
    private float _deltaX;

    private int _playerId;
    private int _currentLevelNumber;
    
    private Vector2 _visualShift = new(
        Level.InitialPos.X * Level.TileSize - Level.TileSize,
        Level.InitialPos.Y * Level.TileSize - Level.TileSize);

    public GameCycleView()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += Window_ClientSizeChanged;
        _graphics.IsFullScreen = false;
        
        var adapter = GraphicsAdapter.DefaultAdapter;
        _graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width / 2;
        _graphics.PreferredBackBufferHeight = 2 * adapter.CurrentDisplayMode.Height / 3;

        _graphics.ApplyChanges();
    }
    
    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
        _graphics.ApplyChanges();
        
        ClientSizeChanged!(this, new ClientSizeEventArgs { Height = _graphics.PreferredBackBufferHeight });
    }

    public void UpdateLevelState(bool levelFinished, GameState gameState)
    {
        _currentGameState = gameState;
        if (_currentGameState == GameState.Game)
            LoadAnimations();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _textures.Add(0, Content.Load<Texture2D>("player"));
        _textures.Add(1, Content.Load<Texture2D>("floorBlock"));
        _textures.Add(2, Content.Load<Texture2D>("wallBlock"));
        _textures.Add(3, Content.Load<Texture2D>("monster"));
        _textures.Add(4, Content.Load<Texture2D>("playerBullet"));
        _textures.Add(5, Content.Load<Texture2D>("monsterBullet"));
        _textures.Add(6, Content.Load<Texture2D>("mainMenuBackground"));
        _textures.Add(7, Content.Load<Texture2D>("heart"));
        _textures.Add(8, Content.Load<Texture2D>("brokenHeart"));
        _textures.Add(9, Content.Load<Texture2D>("skull"));
        _buttonFont = Content.Load<SpriteFont>("montserrat");
    }
    
    private void LoadAnimations()
    {
        _animations.Clear();
        
        var playerAnimation = new Animation(new Point(3, 4), 125, 50, 50);
        playerAnimation.SetLastFrameY(0);
        _animations.Add(_playerId, playerAnimation);
    } 
    
    public void LoadGameCycleParameters(Dictionary<int, IEntity> entities, Vector2 POVShift, GameState currentGameState, 
        int playerId, int currentLevelNumber)
    {
        _playerId = playerId;
        _entities = entities;
        _currentGameState = currentGameState;
        _visualShift += POVShift;
        _currentLevelNumber = currentLevelNumber;
    }

    protected override void Update(GameTime gameTime)
    {
        foreach (var animation in _animations.Values) 
            animation.Update(gameTime);
        
        var directions = new List<Direction>();
        
        var playerAnimation = _currentGameState == GameState.Game ? _animations[_playerId] : null;
        
        
        var keysState = Keyboard.GetState();
        var pressedKeys = keysState.GetPressedKeys().Reverse();

        CheckMenuClickableButtons();
        foreach (var key in pressedKeys)
        {
            if(_currentGameState == GameState.Game)
                CheckInGameButtons(key, directions, playerAnimation);
            else
                CheckInMenuButtons(key);
        }

        if (playerAnimation != null)
        {
            if (directions.Count > 0)
                playerAnimation.SetLastFrameY(playerAnimation.CurrentFrame.Y);
            else
                playerAnimation.SetCurrentFrameX(0);
        }

        base.Update(gameTime);
        CycleFinished!(this, EventArgs.Empty);
    }

    private void CheckInGameButtons(Keys key, List<Direction> directions, Animation playerAnimation)
    {
        switch (key)
        {
            case Keys.W:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.North });
                directions.Add(Direction.North);
                playerAnimation?.SetCurrentFrameY(0);

                break;
            case Keys.S:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.South });
                directions.Add(Direction.South);
                playerAnimation?.SetCurrentFrameY(2); 
                
                break;
            case Keys.D:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.East });
                directions.Add(Direction.East);
                playerAnimation?.SetCurrentFrameY(1); 
                
                break;
            case Keys.A:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.West });
                directions.Add(Direction.West);
                playerAnimation?.SetCurrentFrameY(3); 
                
                break;
                
            
            case Keys.Up:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.North });
                playerAnimation?.SetCurrentFrameY(0);
                
                break;
            case Keys.Right:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.East });
                playerAnimation?.SetCurrentFrameY(1); 
                
                break;
            case Keys.Left:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.West });
                playerAnimation?.SetCurrentFrameY(3);
                
                break;
            case Keys.Down:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.South });
                playerAnimation?.SetCurrentFrameY(2); 
                
                break;
                
            case Keys.Escape:
                if ((DateTime.Now - _lastTimeExitButtonPressed).Milliseconds < 200) 
                    break;
                
                ChangedGameState!(this, EventArgs.Empty);
                _lastTimeExitButtonPressed = DateTime.Now;
                break;
        }
    }
    
    private void CheckInMenuButtons(Keys key)
    {
        if ((DateTime.Now - _lastTimeExitButtonPressed).Milliseconds < 200 || _playerId == 0) 
            return;
        
        if (key == Keys.Escape) 
            ChangedGameState!(this, EventArgs.Empty);
        
        _lastTimeExitButtonPressed = DateTime.Now;
    }

    private void CheckMenuClickableButtons()
    {
        if (_currentGameState != GameState.Menu) return;
        
        var buttons = _entities.Values.Cast<Button>().ToArray();
        
        if (buttons[1].IsClicked())
            Environment.Exit(0);
        else if (buttons[0].IsClicked())
        {
            StartNewGame!(this, EventArgs.Empty);
            buttons[0].Clicked();
        }
    }
}