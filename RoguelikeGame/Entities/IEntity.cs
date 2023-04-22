using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public interface IEntity
{
    int ImageId { get; }
    Vector2 Position { get; set; }

    void Update();
}