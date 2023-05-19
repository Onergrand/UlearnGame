using RoguelikeGame.Creatures;

namespace RoguelikeGame.Entities.Creatures;

public interface ICreature : IEntity, ISolid
{
    int HealthPoints { get; set; }
    int Damage { get; set; }
    int ArmorPoints { get; set; }

    void ApplyDamage(int damage);
}