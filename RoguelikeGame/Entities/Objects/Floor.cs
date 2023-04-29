using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures.Objects;

public class Floor : IEntity
{
    public int ImageId { get; }
    public Vector2 Position { get; set; }

    public Floor(int imageId, Vector2 position)
    {
        ImageId = imageId;
        Position = position;
    }

    public void Update()
    {
        
    }
}