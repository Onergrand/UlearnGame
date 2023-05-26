using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame.Entities.Objects;

public class Floor : IEntity
{
    public int ImageId { get; }
    public int Id { get; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }

    public Floor(int imageId, Vector2 position, int id)
    {
        ImageId = imageId;
        Position = position;
        Id = id;
    }
    
    public void Update() { }
}