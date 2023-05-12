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
    
    private void CheckCollision(IEnumerable<IEntity> currentEntities)
    {
        var enumerable = currentEntities.ToArray();
        var entities = enumerable.Where(x => x is not RoguelikeGame.Entities.Creatures.Player).ToArray();
        var creatures = enumerable.Where(x => x is ICreature).Select(x => ((ICreature)x, x.Position)).ToArray();
        Player.Update();

        foreach (var entity in entities)
        {
            entity.Update();
            var solid = entity as ISolid;

            if (entity is Enemy monster)
            {
                monster.Attack(Player.Position, Entities);
                monster.MoveToPlayer(Player.Position);
            }

            foreach (var creaturePair in creatures)
            {
                var creature = creaturePair.Item1;
                var creatureInitPos = creaturePair.Position;
                if (RectangleCollider.IsCollided(solid!.Collider, creature.Collider))
                {
                    if (creatureInitPos != creature.Speed)
                    {
                        var intersects = Rectangle.Intersect(creature.Collider.Boundary, solid.Collider.Boundary);
                        if (intersects.Width > intersects.Height)
                            creature.Speed = new Vector2(creature.Position.X, creatureInitPos.Y);
                        else if (intersects.Width < intersects.Height)
                            creature.Speed = new Vector2(creatureInitPos.X, creature.Position.Y);
                        creature.Update();
                    }
                }   
            }
        }
    }

    private void CheckBulletCollision(Dictionary<int,IEntity> currentEntities)
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