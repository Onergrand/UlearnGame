using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public class Enemy : ICreature
{
    public int ImageId { get; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    public int HealthPoints { get; set; }
    
    public Enemy(int imageId, Vector2 position, int healthPoints = 500)
    {
        ImageId = imageId;
        HealthPoints = healthPoints;
        Position = position;
        Speed = Position;
    }
    
    public void Update()
    {
    }
    
    public void Attack()
    {
        throw new System.NotImplementedException();
    }
}