using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures.Objects;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Creatures;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.LevelGeneration;

namespace RoguelikeGame.GameModel;

public partial class GameCycle
{
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
        
        previousRoom.PlayerIsOutsideRoom -= ChangeCurrentRoomIfExited;
        _currentRoom.PlayerIsOutsideRoom += ChangeCurrentRoomIfExited;
    }
    
    public void Update()
    {
        _currentRoom.IsPlayerInRoomBounds(Player.Position);

        var currentEntities = Entities
            .Values
            .Where(entity => _currentRoom.IsPositionInRoomBounds(entity.Position) && entity is ISolid);
        
        var currentEntitiesWithKeys = Entities
            .Where(entity => 
                _currentRoom.IsPositionInRoomBounds(entity.Value.Position) && 
                entity.Value is ISolid)
            .ToDictionary(x => x.Key, y => y.Value);

        CheckBulletsCollision(currentEntitiesWithKeys);
        CheckCollision(currentEntities);

        var playerShift = _currentPov;
        _currentPov = new Vector2(0, 0);
        
        Updated!(this, new GameEventArgs { Entities = Entities, POVShift = playerShift});                  
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

    private static void CheckCreatureCollision(IEntity entity, ICreature creature, Vector2 creatureInitialPos)
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
                        
                        creature.ApplyDamage(bullet.Damage);
                        if (creature.HealthPoints <= 0)
                        {
                            Entities.Remove(entityId);
                            if (creature is Player)
                                Environment.Exit(0);
                        }
                    }
                }
                
                if (!_currentRoom.IsPositionInRoomBounds(bullet.Position))
                    Entities.Remove(bulletId);
            }
        }
    }
}