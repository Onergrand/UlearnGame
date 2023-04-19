using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public interface ICreature : IEntity
{
    int HealthPoints { get; set; }
    
    Vector2 Speed { get; set; }

    void Attack();
}