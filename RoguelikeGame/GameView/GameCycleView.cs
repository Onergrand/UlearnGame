﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RoguelikeGame.Creatures;
using RoguelikeGame.Creatures.Objects;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel;
using RoguelikeGame.GameModel.LevelGeneration;

namespace RoguelikeGame.GameView;


public class GameCycleView : Game, IGameView
{
    public event EventHandler CycleFinished;
    public event EventHandler<ControlsEventArgs> PlayerMoved;
    public event EventHandler<ControlsEventArgs> PlayerAttacked;
    
    private Dictionary<int, IEntity> _entities = new();
    private readonly Dictionary<int, Texture2D> _textures = new();
    
    private readonly GraphicsDeviceManager _graphics;
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
        _textures.Add(5, Content.Load<Texture2D>("monsterBullet"));
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

        foreach (var key in pressedKeys)
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
                        Exit();
                    break;
            }
        }
        
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
        
        var walls = _entities.Values.Where(x => x is Wall);
        foreach (var o in _entities.Values.Except(floor))
            _spriteBatch.Draw(_textures[o.ImageId], o.Position - _visualShift, Color.White);
        
        foreach (var o in _entities.Values.Except(floor).Except(walls))
            _spriteBatch.Draw(_textures[o.ImageId], o.Position - _visualShift, Color.White);

        _spriteBatch.End();            
    } 
}