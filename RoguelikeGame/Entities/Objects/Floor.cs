using Microsoft.Xna.Framework;

namespace RoguelikeGame.Entities.Objects;

public class Floor : IEntity
{
    public int ImageId { get; }
    public int Id { get; }
    public Vector2 Position { get; set; }

    public Floor(int imageId, Vector2 position, int id)
    {
        ImageId = imageId;
        Position = position;
        Id = id;
    }
    
    public void Update() { }
}