using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RoguelikeGame.Entities.Objects;
using RoguelikeGame.GameModel.Helpers;

namespace RoguelikeGame.Entities.Creatures;


public static class EnemyType
{
    public enum MonsterType
    {
        Default,
        Spike,
        Tank,
        Strong
    }

    public static Enemy CreateNewEnemy(Vector2 position, int id)
    {
        var rnd = new Random();
        var monsterTypes = Enum.GetValues(typeof(MonsterType));
        var monsterType = (MonsterType)monsterTypes.GetValue(rnd.Next(4))!;
        
        var monster = monsterType switch
        {
            MonsterType.Default => new Enemy(3, position, MonsterType.Default, id),
            MonsterType.Spike => new Enemy(3, position, MonsterType.Spike, id),
            MonsterType.Tank => new Enemy(3, position, MonsterType.Tank, id, 1000),
            MonsterType.Strong => new Enemy(3, position, 600, 200, 1, id, MonsterType.Strong)
        };

        return monster;
    }
    
    
    public static void DefaultAttack(Enemy enemy, Vector2 playerPosition, Dictionary<int, IEntity> entities)
    {
        if ((DateTime.Now - enemy.LastShotTime).Milliseconds < 550)
            return;
        
        enemy.LastShotTime = DateTime.Now;
        
        var bulletStartPosition = enemy.Position + new Vector2(25, 25) + enemy.Position.GetDirectionToPosition(playerPosition) * 50;

        var id = entities.Keys.Max() + 1;
        
        var bullet = new Bullet(
            5,
            bulletStartPosition,
            bulletStartPosition.GetDirectionToPosition(playerPosition) * 5,
            enemy.Damage,
            id);
        
        entities.Add(id, bullet);
    }
    
    public static void SpikeAttack(Enemy enemy, Dictionary<int, IEntity> entities)
    {
        if ((DateTime.Now - enemy.LastShotTime).Seconds < 1)
            return;
        
        enemy.LastShotTime = DateTime.Now;

        
        var directionsValues = (Direction[])Enum.GetValues(typeof(Direction));
        var directions = new List<Direction>();
        directions.AddRange(directionsValues.Except(new[] { Direction.Null }));
        
        var id = entities.Keys.Max() + 1;
        
        var bullets = directions
            .Select(direction => (enemy.Position + new Vector2(25, 25) + direction.ConvertToVector() * 50, direction))
            .Select(startPosition => new Bullet(
                5,
                startPosition.Item1,
                startPosition.direction.ConvertToVector() * 5,
                enemy.Damage, 
                id++))
            .ToList();
        
        foreach (var bullet in bullets)
            entities.Add(bullet.Id, bullet);
    }
}