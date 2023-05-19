using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame.Entities.Objects;

public class Wall : IEntity, ISolid
{
    public int ImageId { get; }
    public RectangleCollider Collider { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }

    public Wall(int imageId, Vector2 position)
    {
        ImageId = imageId;
        Position = position;
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
    }
    
    public void MoveCollider(Vector2 newPos)
    {
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 50, 50);
    }
    
    public void Update()
    {
        MoveCollider(Position);
    }
}