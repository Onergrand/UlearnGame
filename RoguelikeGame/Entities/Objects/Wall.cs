using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures.Objects;

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
        Collider = new RectangleCollider((int)Position.X, (int)Position.Y, 48, 48);
    }
    
    public void MoveCollider(Vector2 newPos)
    {
    }
    
    public void Update()
    {
        MoveCollider(Position);
    }
}