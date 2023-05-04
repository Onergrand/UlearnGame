using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public interface IEntity
{
    int ImageId { get; }
    Vector2 Position { get; set; }

    Vector2 Speed { get; set; }
    void Update();
}