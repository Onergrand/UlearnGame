using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public class Enemy : ICreature
{
    public int Id { get; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    public int HealthPoints { get; set; }
    
    public Enemy(int id, Vector2 position, Vector2 speed, int healthPoints = 500)
    {
        Id = id;
        HealthPoints = healthPoints;
        Position = position;
        Speed = speed;
    }
    
    public void Update()
    {
        throw new System.NotImplementedException();
    }
    
    public void Attack()
    {
        throw new System.NotImplementedException();
    }
}