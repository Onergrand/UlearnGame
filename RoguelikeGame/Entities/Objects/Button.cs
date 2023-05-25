using Microsoft.Xna.Framework;

namespace RoguelikeGame.Entities.Objects;

public class Button : IEntity, ISolid
{
    public int ImageId { get; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    public RectangleCollider Collider { get; set; }
    
    public Button(int imageId, Vector2 position, int buttonWidth, int buttonHeight)
    {
        ImageId = imageId;
        Position = position;
        Collider = new RectangleCollider((int)position.X, (int)position.Y, buttonWidth, buttonHeight);
    }
    
    public void Update() { }
    public void MoveCollider(Vector2 newPos) { }
}