using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RoguelikeGame.Entities.Creatures;

public interface IEnemy : ICreature
{
    EnemyType.MonsterType EnemyBehaviour { get; set; }
    void Attack(Vector2 playerPosition, Dictionary<int, IEntity> entities);
}