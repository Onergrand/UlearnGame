﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.Helpers;
using RoguelikeGame.GameModel.LevelGeneration;

namespace RoguelikeGame.GameModel;

public partial class GameCycle
{
    public event EventHandler<LevelStateArgs> UpdateLevelState;
    
    private Vector2 _cameraDeltaPosition = Vector2.Zero;
    private int _remainingMonstersAmount;
    private Vector2 _lastKnownPlayerSpeed;
    private Vector2 _currentPov = new(0,0);

    private void InitializeGame(int currentLevelNumber, int enemyKilled)
    {
        _currentLevelNumber = currentLevelNumber;
        Entities = new Dictionary<int, IEntity>();
        CreateLevel(currentLevelNumber);

        var player = new Player(0, 
            new Vector2(Level.InitialPos.X * Level.TileSize, Level.InitialPos.Y * Level.TileSize), _currentId);
        
        player.EnemyKilled += enemyKilled;
        Entities.Add(_currentId, player);
        
        Player = player;
        _currentId++;
        
        UpdateGame();
        UpdateLevelState(this, new LevelStateArgs{ LevelFinished = false, GameState = _currentGameState});
    }
    
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
                if (cell == RoomObject.Empty)
                    continue;
                
                var pos = new Vector2(i * Level.TileSize - Level.TileSize, j * Level.TileSize - Level.TileSize);
                FillCell(cell, pos);
            }
        }
    }

    private void FillCell(RoomObject cell, Vector2 pos)
    {
        switch (cell)
        {
            case RoomObject.Wall:
                Entities.Add(_currentId, new Wall(2, pos, _currentId));
                _currentId++;
                break;

            case RoomObject.Monster:
                Entities.Add(_currentId, new Floor(1, pos, _currentId));
                _currentId++;
                Entities.Add(_currentId, EnemyType.CreateNewEnemy(pos, _currentId));
                _currentId++;
                break;

            case RoomObject.Floor:
            case RoomObject.Player:
            case RoomObject.Exit:
                Entities.Add(_currentId, new Floor(1, pos, _currentId));
                _currentId++;
                break;
        }
    }
    
    private void ChangeCurrentRoomIfExited()
    {
        var previousRoom = _currentRoom;
        foreach (var neighbour in _currentRoom.Neighbours.Values)
        {
            if (!neighbour.IsPositionInRoomBounds(Player.Position)) 
                continue;
            
            _currentRoom = neighbour;
            break;
        }
        if (previousRoom == _currentRoom)
            throw new ArgumentOutOfRangeException($"Player is outside the map bounds");

        var delta = _currentRoom.TopLeftCorner - previousRoom.TopLeftCorner;
        _currentPov = new Vector2(delta.X * Level.TileSize, delta.Y * Level.TileSize);
        _cameraDeltaPosition -= _currentPov;

        previousRoom.PlayerIsOutsideRoom -= ChangeCurrentRoomIfExited;
        _currentRoom.PlayerIsOutsideRoom += ChangeCurrentRoomIfExited;
    }

    private void UpdateGame()
    {
        _currentRoom.IsPlayerInRoomBounds(Player.Position);

        var currentEntities = Entities.Where(x => _currentRoom.IsPositionInRoomBounds(x.Value.Position))
            .ToDictionary(x => x.Key, y => y.Value);

        var currentSolidEntities = currentEntities.Values.Where(entity => entity is ISolid);
        
        var currentEntitiesWithKeys = currentEntities.Where(entity => entity.Value is ISolid)
            .ToDictionary(x => x.Key, y => y.Value);

        CheckBulletsCollision(currentEntitiesWithKeys);
        
        if (_currentGameState == GameState.Menu) return;
        
        CheckCollision(currentSolidEntities);

        var playerShift = _currentPov;
        _currentPov = new Vector2(0, 0);

        Updated!(this, new GameEventArgs
        {
            Entities = currentEntities,
            POVShift = playerShift,
            CurrentGameState = _currentGameState,
            PlayerId = Player.Id,
            CurrentLevelNumber = _currentLevelNumber
        });
    }
    
    private void CheckCollision(IEnumerable<IEntity> currentEntities)
    {
        _lastKnownPlayerSpeed = Player.Speed;
        var enumerable = currentEntities.ToArray();
        var entities = enumerable.Where(x => x is not RoguelikeGame.Entities.Creatures.Player).ToArray();
        var creatures = enumerable.Where(x => x is ICreature).Select(x => ((ICreature)x, x.Position)).ToArray();

        foreach (var creature in creatures)
        {
            if (creature.Item1 is Enemy monster)
            {
                monster.MoveToPlayer(Player.Position);
                monster.Attack(Player.Position, Entities);
            }
            creature.Item1.Update();
        }
        Player.Update();

        foreach (var entity in entities)
        {
            entity.Update();

            foreach (var (creature, creatureInitialPos) in creatures)
                CheckCreatureCollision(entity, creature, creatureInitialPos);
        }
    }

    private void CheckCreatureCollision(IEntity entity, ICreature creature, Vector2 creatureInitialPos)
    {
        if (entity.Equals(creature)) return;

        var solid = entity as ISolid;
        
        if (!RectangleCollider.IsCollided(solid!.Collider, creature.Collider)) return;
        if (creatureInitialPos == creature.Position)
            return;
        
        var intersects = Rectangle.Intersect(creature.Collider.Boundary, solid.Collider.Boundary);
        if (intersects.Width >= intersects.Height)
            creature.Position = new Vector2(creature.Position.X, creatureInitialPos.Y);
        else if (intersects.Width < intersects.Height)
            creature.Position = new Vector2(creatureInitialPos.X, creature.Position.Y);
        
        creature.Update();
    }

    private void CheckBulletsCollision(Dictionary<int,IEntity> currentEntities)
    {
        var bullets = currentEntities
            .Where(x => x.Value is Bullet)
            .ToDictionary(x => x.Key, y => y.Value);
        
        foreach (var (bulletId, value) in bullets)
        {
            var bullet = value as Bullet;
            
            foreach (var (entityId, entity) in currentEntities.Where(x => !bullets.ContainsKey(x.Key)))
            {
                var solid = entity as ISolid;
                if (RectangleCollider.IsCollided(solid!.Collider, bullet!.Collider))
                {
                    Entities.Remove(bulletId);
                    
                    if (solid is ICreature creature)
                    {
                        if (bullet.ImageId == 5 && creature is Enemy)
                            continue;

                        if (HandleCreatureDamageAndCheckLevelCompletion(creature, bullet, entityId)) return;
                    }
                }
                
                if (!_currentRoom.IsPositionInRoomBounds(bullet.Position))
                    Entities.Remove(bulletId);
            }
        }
    }

    private bool HandleCreatureDamageAndCheckLevelCompletion(ICreature creature, Bullet bullet, int entityId)
    {
        creature.ApplyDamage(bullet.Damage);
        if (creature.HealthPoints > 0) return false;
        
        Entities.Remove(entityId);
        _remainingMonstersAmount--;
        Player.EnemyKilled++;


        if (creature is Player)
        {
            ChangeGameState();
            UpdateLevelState!(this, new LevelStateArgs { LevelFinished = true, GameState = _currentGameState });

            return true;
        }

        if (_remainingMonstersAmount != 0) return false;
        
        _currentPov = _cameraDeltaPosition;
        _cameraDeltaPosition = Vector2.Zero;
        InitializeGame(_currentLevelNumber + 1, Player.EnemyKilled);

        return false;
    }
}