using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.GameView;

public partial class GameCycleView
{
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

        _backgroundScaling = GetScaling(_textures[6].Height);
        _deltaX = GetDeltaX(_textures[6].Width, _backgroundScaling);

        DrawTexture(6, Vector2.Zero);
        
        DrawButton(_entities[0] as Button);
        DrawButton(_entities[1] as Button);
        
        _spriteBatch.DrawString(_buttonFont, "Shadowed Abyss",
            new Vector2(800 + _deltaX, (float)_graphics.PreferredBackBufferHeight / 16), Color.White);
        
        _spriteBatch.End(); 
    }

    private void DrawGame(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        _backgroundScaling = GetScaling(500);
        _deltaX = GetDeltaX(600, _backgroundScaling);
        
        base.Draw(gameTime);
        _spriteBatch.Begin();

        var floor = _entities.Values.Where(x => x is Floor).ToArray();
        var walls = _entities.Values.Where(x => x is Wall).ToArray();

        DrawEntitiesInOrder(floor, walls, _entities.Values.Except(floor).Except(walls));
        DrawHUD();
        
        _spriteBatch.End();   
    }

    private void DrawHUD()
    {
        if (!_entities.ContainsKey(_playerId)) return;
        
        var player = (Player)_entities[_playerId];
        var playerHP = player.HealthPoints;
        var heartsAmount = playerHP / 100;
        var halfHeartsAmount = (int)Math.Round((playerHP - heartsAmount * 100) / 100.0);

        if (heartsAmount == 0)
            halfHeartsAmount = 1;
        
        for (var i = 0; i < heartsAmount + halfHeartsAmount; i++)
        {
            var currentX = 10 + 40 * i;
            DrawTexture(i == heartsAmount + halfHeartsAmount - 1 && halfHeartsAmount != 0 ? 8 : 7, new Vector2(currentX, 10));
        }
        DrawTexture(9, new Vector2(300, 10));
        
        _spriteBatch.DrawString(_buttonFont, $"x{player.EnemyKilled}",
            new Vector2(325 * _backgroundScaling + _deltaX, 5), Color.White);
        _spriteBatch.DrawString(_buttonFont, $"Level:{_currentLevelNumber}",
            new Vector2(400 * _backgroundScaling + _deltaX, 5), Color.White);   
    }

    private void DrawEntitiesInOrder(params IEnumerable<IEntity>[] entitiesCollection)
    {
        foreach (var collection in entitiesCollection)
        foreach (var entity in collection)
        {
            if (_animations.ContainsKey(entity.Id))
                DrawAnimation(entity);
            else
                DrawTexture(entity.ImageId, entity.Position - _visualShift);
        }
    }

    private void DrawAnimation(IEntity entity)
    {
        var animation = _animations[entity.Id];
        var texturePosition = GetEntityPositionByWindowSize(entity.Position - _visualShift);

        _spriteBatch.Draw(
            _textures[entity.ImageId],
            texturePosition,
            new Rectangle(
                animation.CurrentFrame.X * animation.FrameWidth,
                animation.CurrentFrame.Y * animation.FrameHeight,
                animation.FrameWidth, animation.FrameHeight),
            Color.White,
            0,
            Vector2.Zero,
            _backgroundScaling,
            SpriteEffects.None,
            1.0F);
    }

    private void DrawTexture(int textureId, Vector2 position)
    {
        var texturePosition = GetEntityPositionByWindowSize(position);

        _spriteBatch.Draw(_textures[textureId], 
            texturePosition, 
            null,
            Color.White, 
            0.0F, 
            Vector2.Zero, 
            _backgroundScaling, 
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

    private Vector2 GetEntityPositionByWindowSize(Vector2 position)
    {
        return position * _backgroundScaling + new Vector2(_deltaX, 0);
    }
}