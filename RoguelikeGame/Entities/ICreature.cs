namespace RoguelikeGame.Entities;

public interface ICreature : IEntity, ISolid
{
    int HealthPoints { get; }
    int Damage { get; set; }
    int ArmorPoints { get; set; }

    void ApplyDamage(int damage);
}