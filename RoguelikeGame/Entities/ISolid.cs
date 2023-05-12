using Microsoft.Xna.Framework;
using RoguelikeGame.Entities.Objects;

namespace RoguelikeGame.Entities;

public interface ISolid
{
    RectangleCollider Collider { get; set; }
    
    void MoveCollider(Vector2 newPos);
}