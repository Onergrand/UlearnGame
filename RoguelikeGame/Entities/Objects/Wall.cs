using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures.Objects;

public class Wall : IEntity
{
    public int ImageId { get; }
    public Vector2 Position { get; set; }
    
    public Wall(int imageId, Vector2 position)
    {
        ImageId = imageId;
        Position = position;
    }
    
    public void Update()
    {
        
    }
}