using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame.Entities.Creatures;

public class Player : ICreature, ISolid
{
    public int ImageId { get; }
    public int HealthPoints { get; set; }
    public RectangleCollider Collider { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }

    public Direction Direction;


    public Player(int imageId, Vector2 position, int healthPoints = 500)
    {
        ImageId = imageId;
        HealthPoints = healthPoints;
        Position = position;
        Speed = Position;
        Collider = new RectangleCollider((int)Speed.X, (int)Speed.Y, 50, 50);
    }

    public void Update()
    {
        Position = Speed;
        MoveCollider(Position);
    }
    
    public void Attack()
    {
        
    }
    
    public void MoveCollider(Vector2 newPos)
    {
        Collider = new RectangleCollider((int)Speed.X, (int)Speed.Y, 50, 50);
    }
}