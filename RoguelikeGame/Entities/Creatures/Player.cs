using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public class Player : ICreature
{
    public int Id { get; }
    public int HealthPoints { get; set; }

    public Vector2 Position { get; set; } = new(250, 300);
    public Vector2 Speed { get; set; } = new(0, 0);


    public Player(int id, Vector2 position, Vector2 speed, int healthPoints = 500)
    {
        Id = id;
        HealthPoints = healthPoints;
        Position = position;
        Speed = speed;
    }

    public void Update()
    {
        Position = Speed;
    }
    
    public void Attack()
    {
        
    }
}