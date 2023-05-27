using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities.Creatures;

namespace RoguelikeGame.Entities;

public interface IEnemy : ICreature
{
    EnemyType.MonsterType EnemyBehaviour { get; set; }
    void Attack(Vector2 playerPosition, Dictionary<int, IEntity> entities);
}