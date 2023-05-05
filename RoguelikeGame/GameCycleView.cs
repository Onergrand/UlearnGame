using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoguelikeGame.Creatures;
using RoguelikeGame.Creatures.Objects;
using RoguelikeGame.LevelGeneration;

namespace RoguelikeGame;

public class GameCycleView : Game, IGameView
{
    public event EventHandler CycleFinished;
    public event EventHandler<ControlsEventArgs> PlayerMoved;
    public event EventHandler PlayerAttacked;
    
    private Dictionary<int, IEntity> _entities = new();
    private Dictionary<int, Texture2D> _textures = new();
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Vector2 _visualShift = new(
        Level.InitialPos.X * Level.TileSize - 50,
        Level.InitialPos.Y * Level.TileSize - 50);

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
        _textures.Add(3, Content.Load<Texture2D>("monster"));
        _textures.Add(4, Content.Load<Texture2D>("playerBullet"));
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
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.NorthWest });
                
            else if (keysState.IsKeyDown(Keys.W) && keysState.IsKeyDown(Keys.D))
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.NorthEast });
                
            else if (keysState.IsKeyDown(Keys.S) && keysState.IsKeyDown(Keys.D))
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.SouthEast });
                
            else if (keysState.IsKeyDown(Keys.S) && keysState.IsKeyDown(Keys.A))
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.SouthWest });
            
            else if (keysState.IsKeyDown(Keys.W))
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.North });
                
            else if (keysState.IsKeyDown(Keys.S))
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.South });
                
            else if (keysState.IsKeyDown(Keys.D))
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.East });
                
            else if (keysState.IsKeyDown(Keys.A))
                PlayerMoved!(this, new ControlsEventArgs { Direction = Direction.West });
                
            else if (keysState.IsKeyDown(Keys.Escape)) 
                Exit();
        }
        
        if (keysState.IsKeyDown(Keys.Space))
            PlayerAttacked!(this, EventArgs.Empty);

        base.Update(gameTime);
        CycleFinished!(this, EventArgs.Empty);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(177, 180, 186));
        base.Draw(gameTime);
        _spriteBatch.Begin();

        var floor = _entities.Values.Where(x => x is Floor);
        foreach (var o in floor)
            _spriteBatch.Draw(_textures[o.ImageId], o.Position - _visualShift, Color.White);
        
        foreach (var o in _entities.Values.Except(floor))
            _spriteBatch.Draw(_textures[o.ImageId], o.Position - _visualShift, Color.White);

        _spriteBatch.End();            
    } 
}