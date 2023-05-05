using System;
using Microsoft.Xna.Framework;
using RoguelikeGame.Creatures;

namespace RoguelikeGame.Entities.Creatures;

public class Enemy : ICreature, ISolid
{
    public int ImageId { get; }
    public Vector2 Position { get; set; }
    public Vector2 Speed { get; set; }
    public int HealthPoints { get; set; }
    public RectangleCollider Collider { get; set; }
    
    public Enemy(int imageId, Vector2 position, int healthPoints = 500)
    {
        ImageId = imageId;
        HealthPoints = healthPoints;
        Position = position;
        Speed = Position;
        Collider = new RectangleCollider((int)Speed.X + 15, (int)Speed.Y + 15, 30, 30);
    }
    
    public void Update()
    {
        Position = Speed;
        MoveCollider(Position);
    }
    
    public void Attack()
    {
        throw new System.NotImplementedException();
    }
    
    public void MoveCollider(Vector2 newPos)
    {
        Collider = new RectangleCollider((int)Speed.X + 15, (int)Speed.Y + 15, 30, 30);
    }

    public void MoveToPlayer(Vector2 playerPosition)
    {
        if (Position.GetDistanceTo(playerPosition) < 90) 
            return;

        Speed += Position.GetDirectionToPosition(playerPosition) * 3;
    }
}