using System;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel;

namespace RoguelikeGame.Entities.Creatures;

public class Player : ICreature
{
    public int ImageId { get; }
    public int Id { get; }
    public int HealthPoints { get; set; }
    public int Damage { get; set; } = 170;
    public int ArmorPoints { get; set; } = 4;
    


    public RectangleCollider Collider { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    
    public DateTime LastShotTime = DateTime.Now;



    public Player(int imageId, Vector2 position, int id, int healthPoints = 500)
    {
        ImageId = imageId;
        HealthPoints = healthPoints;
        Position = position;
        Id = id;
        
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
    }

    public void Update()
    {
        Position += Speed;
        MoveCollider(Position);
        Speed = new Vector2(0, 0);
    }
    
    public void MoveCollider(Vector2 newPos)
    {
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
    }
    
    public void ApplyDamage(int damage) => HealthPoints -= (int)(damage / (0.34 * ArmorPoints));
}