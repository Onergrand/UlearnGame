using Microsoft.Xna.Framework;
using RoguelikeGame.Entities;
using RoguelikeGame.Entities.Objects;

namespace RoguelikeGame.Creatures.Objects;

public class Bullet : IMissile, ISolid
{
    public int ImageId { get; }
    
    public RectangleCollider Collider { get; set; }
    
    public int Damage { get; set; }
    
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }

    public Bullet(int imageId, Vector2 position, Vector2 speed, int damage)
    {
        ImageId = imageId;
        Position = position;
        Speed = speed;
        Damage = damage;
        Collider = new RectangleCollider((int)position.X, (int)position.Y, 20, 20);
    }
    
    public void Update()
    {
        Position += Speed;
        MoveCollider(Position);
    }
    
    public void MoveCollider(Vector2 newPos)
    {
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 20, 20);
    }
}