using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public class Player : ICreature
{
    public int ImageId { get; }
    public int HealthPoints { get; set; }

    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }


    public Player(int imageId, Vector2 position, int healthPoints = 500)
    {
        ImageId = imageId;
        HealthPoints = healthPoints;
        Position = position;
        Speed = Position;
    }

    public void Update()
    {
        Position = Speed;
    }
    
    public void Attack()
    {
        
    }
}