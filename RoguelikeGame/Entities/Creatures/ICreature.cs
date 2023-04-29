using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public interface ICreature : IEntity
{
    int HealthPoints { get; set; }

    void Attack();
}