using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures.Objects;

public class Bullet : IEntity, ISolid
{
    public int ImageId { get; }
    public RectangleCollider Collider { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }

    public Bullet(int imageId, Vector2 position, Vector2 speed)
    {
        ImageId = imageId;
        Position = position;
        Speed = speed;
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