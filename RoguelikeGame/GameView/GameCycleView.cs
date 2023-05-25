using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.Helpers;
using RoguelikeGame.GameModel.LevelGeneration;

namespace RoguelikeGame.GameView;


public class GameCycleView : Game, IGameView
{
    public event EventHandler CycleFinished;
    public event EventHandler<ControlsEventArgs> PlayerMoved;
    public event EventHandler<ControlsEventArgs> PlayerAttacked;
    public event EventHandler ChangedGameState;
    public event EventHandler StartNewGame;

    private Dictionary<int, IEntity> _entities = new();
    private GameState _currentGameState;
    private readonly Dictionary<int, Texture2D> _textures = new();
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private DateTime _lastTimeExitButtonPressed = DateTime.Now;
    private SpriteFont _buttonFont;
    
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
        _textures.Add(7, Content.Load<Texture2D>("newGameButton"));
        _textures.Add(8, Content.Load<Texture2D>("exitButton"));
        _buttonFont = Content.Load<SpriteFont>("montserrat");
    }
    
    public void LoadGameCycleParameters(Dictionary<int, IEntity> entities, Vector2 POVShift, GameState currentGameState)
    {
        _entities = entities;
        _currentGameState = currentGameState;
        _visualShift += POVShift;
    }

    protected override void Update(GameTime gameTime)
    {
        var keysState = Keyboard.GetState();
        var pressedKeys = keysState.GetPressedKeys();

        CheckMenuClickableButtons();
        foreach (var key in pressedKeys)
        {
            if(_currentGameState == GameState.Game)
                CheckInGameButtons(key);
            else
                CheckInMenuButtons(key);
        }

        base.Update(gameTime);
        CycleFinished!(this, EventArgs.Empty);
    }

    private void CheckInGameButtons(Keys key)
    {
        switch (key)
        {
            case Keys.W:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.North });
                break;
            case Keys.S:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.South });
                break;
            case Keys.D:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.East });
                break;
            case Keys.A:
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.West });
                break;
                
            case Keys.Up:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.North });
                break;
            case Keys.Right:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.East });
                break;
            case Keys.Left:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.West });
                break;
            case Keys.Down:
                PlayerAttacked!(this, new ControlsEventArgs { Direction = Direction.South });
                break;
                
            case Keys.Escape:
                if ((DateTime.Now - _lastTimeExitButtonPressed).Milliseconds < 150) 
                    break;
                
                ChangedGameState!(this, EventArgs.Empty);
                _lastTimeExitButtonPressed = DateTime.Now;
                break;
        }
    }
    
    private void CheckInMenuButtons(Keys key)
    {
        if ((DateTime.Now - _lastTimeExitButtonPressed).Milliseconds < 150) return;
        
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

    protected override void Draw(GameTime gameTime)
    {
         if (_currentGameState == GameState.Menu)
             DrawMenu(gameTime);
         else
             DrawGame(gameTime);
    }

    private void DrawMenu(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        base.Draw(gameTime);
        _spriteBatch.Begin();

        var scaleBackground = GetScaling(_textures[6].Height);
        var deltaX = GetDeltaX(_textures[6].Width, scaleBackground);
        
        var startGameButtonPositionX = _graphics.PreferredBackBufferWidth - _textures[7].Width * scaleBackground - 2.3f * deltaX;
        var startGameButtonPositionY = _graphics.PreferredBackBufferHeight / 1.4f;
        var startGameButtonPosition = new Vector2(startGameButtonPositionX, startGameButtonPositionY);
        
        
        DrawTexture(6, Vector2.Zero, scaleBackground, deltaX);
        
        DrawButton(_entities[0] as Button);
        DrawButton(_entities[1] as Button);
        //DrawTexture(7, startGameButtonPosition, scaleBackground, deltaX);
        DrawTexture(8, startGameButtonPosition + new Vector2(0, _textures[8].Height + 20), scaleBackground, deltaX);

        
        _spriteBatch.End(); 
    }

    private void DrawGame(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        var scaleBackground = GetScaling(500);
        var deltaX = GetDeltaX(600, scaleBackground);
        
        base.Draw(gameTime);
        _spriteBatch.Begin();

        var floor = _entities.Values.Where(x => x is Floor).ToArray();
        var walls = _entities.Values.Where(x => x is Wall).ToArray();

        DrawEntitiesInCorrectOrder(scaleBackground, deltaX, floor, walls, _entities.Values.Except(floor).Except(walls));

        _spriteBatch.End();   
    }

    private void DrawEntitiesInCorrectOrder(float scaleBackground, float deltaX, params IEnumerable<IEntity>[] entitiesCollection)
    {
        foreach (var collection in entitiesCollection)
            foreach (var o in collection) 
                DrawTexture(o.ImageId, o.Position - _visualShift, scaleBackground, deltaX);
    }
    
    private void DrawTexture(int textureId, Vector2 position, float scaleBackground, float deltaX)
    {
        var texturePosition = position * scaleBackground + new Vector2(deltaX, 0);

        _spriteBatch.Draw(_textures[textureId], 
            texturePosition, 
            null,
            Color.White, 
            0.0F, 
            Vector2.Zero, 
            scaleBackground, 
            SpriteEffects.None, 
            1.0F);
    }

    private void DrawButton(Button button)
    {
        _spriteBatch.DrawString(_buttonFont, button.Text, button.Position, button.TextColor);
    }

    private float GetScaling(int backgroundHeight)
    {
        return (float)_graphics.PreferredBackBufferHeight / backgroundHeight;
    }

    private float GetDeltaX(int backgroundWidth, float scaleBackground)
    {
        return (_graphics.PreferredBackBufferWidth - backgroundWidth * scaleBackground) / 2;
    }
}