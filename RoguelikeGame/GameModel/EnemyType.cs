using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures.Objects;
using RoguelikeGame.GameModel;

namespace RoguelikeGame.Entities.Creatures;


public class EnemyType
{
    public enum MonsterType
    {
        Default,
        Spike,
        Tank,
        Strong
    }

    public static Enemy CreateNewEnemy(Vector2 position)
    {
        var rnd = new Random();
        var monsterTypes = Enum.GetValues(typeof(MonsterType));
        var monsterType = (MonsterType)monsterTypes.GetValue(rnd.Next(4))!;
        
        var monster = monsterType switch
        {
            MonsterType.Default => new Enemy(3, position, MonsterType.Default),
            MonsterType.Spike => new Enemy(3, position, MonsterType.Spike),
            MonsterType.Tank => new Enemy(3, position, MonsterType.Tank, 1000),
            MonsterType.Strong => new Enemy(3, position, 600, 200, 1, MonsterType.Strong)
        };

        return monster;
    }
    
    
    public static void DefaultAttack(Enemy enemy, Vector2 playerPosition, Dictionary<int, IEntity> entities)
    {
        if ((DateTime.Now - enemy.LastShotTime).Milliseconds < 550)
            return;
        
        enemy.LastShotTime = DateTime.Now;
        
        var bulletStartPosition = enemy.Position + new Vector2(25, 25) + enemy.Position.GetDirectionToPosition(playerPosition) * 50;
        
        var bullet = new Bullet(
            5,
            bulletStartPosition,
            bulletStartPosition.GetDirectionToPosition(playerPosition) * 5,
            enemy.Damage);
        
        entities.Add(entities.Keys.Max() + 1, bullet);
    }
    
    public static void SpikeAttack(Enemy enemy, Dictionary<int, IEntity> entities)
    {
        if ((DateTime.Now - enemy.LastShotTime).Seconds < 1)
            return;
        
        enemy.LastShotTime = DateTime.Now;

        var directions = (Direction[])Enum.GetValues(typeof(Direction));

        var bullets = directions
            .Select(direction => (enemy.Position + new Vector2(25, 25) + direction.ConvertToVector() * 50, direction))
            .Select(startPosition => 
                new Bullet(
                    5,
                    startPosition.Item1,
                    startPosition.direction.ConvertToVector() * 5,
                    enemy.Damage))
            .ToList();

        var currentId = entities.Keys.Max() + 1;
        foreach (var bullet in bullets)
            entities.Add(currentId++, bullet);
    }
}