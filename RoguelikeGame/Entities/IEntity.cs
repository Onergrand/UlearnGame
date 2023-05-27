using Microsoft.Xna.Framework;

namespace RoguelikeGame.Entities;

public interface IEntity
{
    int ImageId { get; }
    int Id { get; }
    Vector2 Position { get; set; }

    void Update();
}