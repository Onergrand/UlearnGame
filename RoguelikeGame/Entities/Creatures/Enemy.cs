using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.Entities.Creatures;

public class Enemy : IEnemy
{
    public int ImageId { get; }
    public int Id { get; }
    public int HealthPoints { get; set; }

    public int Damage { get; set; } = 80;

    public int ArmorPoints { get; set; } = 2;

    public DateTime LastShotTime = DateTime.Now;
    
    public EnemyType.MonsterType EnemyBehaviour { get; set; }
    
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    public RectangleCollider Collider { get; set; }
    
    public Enemy(int imageId, Vector2 position, EnemyType.MonsterType enemyBehaviour, int id, int healthPoints = 500)
    {
        ImageId = imageId;
        HealthPoints = healthPoints;
        Position = position;
        EnemyBehaviour = enemyBehaviour;
        Id = id;
        LastShotTime = DateTime.Now;
        
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
    }
    
    public Enemy(int imageId, Vector2 position, int healthPoints, int damage, int armorPoints, int id, EnemyType.MonsterType enemyBehaviour)
    {
        ImageId = imageId;
        HealthPoints = healthPoints;
        Position = position;
        Damage = damage;
        ArmorPoints = armorPoints;
        EnemyBehaviour = enemyBehaviour;
        Id = id;
        LastShotTime = DateTime.Now;
        
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
    }
    
    public void Update()
    {
        Position += Speed;
        MoveCollider(Position);
        Speed = Vector2.Zero;
    }

    public void Attack(Vector2 playerPosition, Dictionary<int, IEntity> entities)
    {
        if (EnemyBehaviour is EnemyType.MonsterType.Spike) 
            EnemyType.SpikeAttack(this, entities);
        else
            EnemyType.DefaultAttack(this, playerPosition, entities);
    }
    
    public void MoveCollider(Vector2 newPos) => Collider = new RectangleCollider((int)newPos.X, (int)newPos.Y, 50, 50);

    public void MoveToPlayer(Vector2 playerPosition)
    {
        if (Position.GetDistanceTo(playerPosition) < 190) 
            Speed += -Position.GetDirectionToPosition(playerPosition) * 3;
        else if (Position.GetDistanceTo(playerPosition) < 200)
            Speed = new Vector2(0, 0);
        else
            Speed += Position.GetDirectionToPosition(playerPosition) * 3;
        
        MoveCollider(Position + Speed);
    }

    public void ApplyDamage(int damage) => HealthPoints -= (int)(damage / (0.34 * ArmorPoints));
}