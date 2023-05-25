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

    private Dictionary<int, IEntity> _entities = new();
    private GameState _currentGameState;
    private readonly Dictionary<int, Texture2D> _textures = new();
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private DateTime _lastTimeExitButtonPressed = DateTime.Now;
    
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
        _graphics.IsFullScreen = false;
        
        var adapter = GraphicsAdapter.DefaultAdapter;
        _graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width / 2;
        _graphics.PreferredBackBufferHeight = 2 * adapter.CurrentDisplayMode.Height / 3;

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
        var mouseState = Mouse.GetState();
        
        var pressedKeys = keysState.GetPressedKeys();
        var pressedMouseKeys = mouseState.LeftButton;

        foreach (var key in pressedKeys)
        {
            if(_currentGameState == GameState.Game)
                CheckInGameButtons(key);
            else
                CheckInMenuButtons(key);
        }

        if (pressedMouseKeys == ButtonState.Pressed)
        {
            
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
    
    protected override void Draw(GameTime gameTime)
    {
         if (_currentGameState == GameState.Menu)
             DrawMenu(gameTime);
         else
             DrawGame(gameTime);
    }

    private void DrawMenu(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(177, 180, 186));
        base.Draw(gameTime);
        _spriteBatch.Begin();

        var scaleBackground = GetScaling(_textures[6].Height);
        var deltaX = GetDeltaX(_textures[6].Width, scaleBackground);
        
        var startGameButtonPositionX = _graphics.PreferredBackBufferWidth - _textures[7].Width * scaleBackground - 2.3f * deltaX;
        var startGameButtonPositionY = _graphics.PreferredBackBufferHeight / 1.4f;
        var startGameButtonPosition = new Vector2(startGameButtonPositionX, startGameButtonPositionY);
        
        DrawTexture(6, Vector2.Zero, scaleBackground, deltaX);
        DrawTexture(7, startGameButtonPosition, scaleBackground, deltaX);
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
        foreach (var o in floor) 
            DrawTexture(o.ImageId, o.Position - _visualShift, scaleBackground, deltaX);
        

        var walls = _entities.Values.Where(x => x is Wall);
        foreach (var o in _entities.Values.Except(floor)) 
            DrawTexture(o.ImageId, o.Position - _visualShift, scaleBackground, deltaX);

        
        foreach (var o in _entities.Values.Except(floor).Except(walls)) 
            DrawTexture(o.ImageId, o.Position - _visualShift, scaleBackground, deltaX);

        
        _spriteBatch.End();   
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

    private float GetScaling(int backgroundHeight)
    {
        return (float)_graphics.PreferredBackBufferHeight / backgroundHeight;
    }

    private float GetDeltaX(int backgroundWidth, float scaleBackground)
    {
        return (_graphics.PreferredBackBufferWidth - backgroundWidth * scaleBackground) / 2;
    }
}