using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoguelikeGame.Creatures;
using RoguelikeGame.LevelGeneration;

namespace RoguelikeGame;

public class GameCycleView : Game, IGameView
{
    public event EventHandler CycleFinished;
    public event EventHandler<ControlsEventArgs> PlayerMoved;
    
    private Dictionary<int, IEntity> _entities = new();
    private Dictionary<int, Texture2D> _textures = new();
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _playerImage;
    private Vector2 _visualShift = new(
        Level.InitialPos.X * Level.TileSize - 300,
        Level.InitialPos.Y * Level.TileSize - 250);

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
        _graphics.PreferredBackBufferWidth = 600;
        _graphics.PreferredBackBufferHeight = 500;
        _graphics.ApplyChanges();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _textures.Add(0, Content.Load<Texture2D>("player"));
        _textures.Add(1, Content.Load<Texture2D>("floorBlock"));
        _textures.Add(2, Content.Load<Texture2D>("wallBlock"));
    }
    
    public void LoadGameCycleParameters(Dictionary<int, IEntity> entities, Vector2 POVShift)
    {
        _entities = entities;
        _visualShift += POVShift;
    }

    protected override void Update(GameTime gameTime)
    {
        var keysState = Keyboard.GetState();
        var pressedKeys = keysState.GetPressedKeys();
        if (pressedKeys.Length > 0)
        {

            if (keysState.IsKeyDown(Keys.W) && keysState.IsKeyDown(Keys.A))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.NorthWest });
                
            else if (keysState.IsKeyDown(Keys.W) && keysState.IsKeyDown(Keys.D))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.NorthEast });
                
            else if (keysState.IsKeyDown(Keys.S) && keysState.IsKeyDown(Keys.D))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.SouthEast });
                
            else if (keysState.IsKeyDown(Keys.S) && keysState.IsKeyDown(Keys.A))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.SouthWest });
            
            else if (keysState.IsKeyDown(Keys.W))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.North });
                
            else if (keysState.IsKeyDown(Keys.S))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.South });
                
            else if (keysState.IsKeyDown(Keys.D))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.East });
                
            else if (keysState.IsKeyDown(Keys.A))
                PlayerMoved(this, new ControlsEventArgs { Direction = Direction.West });
                
            else if (keysState.IsKeyDown(Keys.Escape)) 
                Exit();
        }

        base.Update(gameTime);
        CycleFinished!(this, new EventArgs());
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(177, 180, 186));
        base.Draw(gameTime);
        _spriteBatch.Begin();
        
        foreach (var o in _entities.Values)
        {
            _spriteBatch.Draw(_textures[o.ImageId], o.Position - _visualShift, Color.White);
        }

        _spriteBatch.End();            
    } 
}