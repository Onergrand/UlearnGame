using Microsoft.Xna.Framework;

namespace RoguelikeGame.Creatures;

public interface ISolid
{
    RectangleCollider Collider { get; set; }
    
    void MoveCollider(Vector2 newPos);
}