﻿using Microsoft.Xna.Framework;

namespace RoguelikeGame.Entities.Objects;

public class RectangleCollider
{
    public Rectangle Boundary { get; }
    
    public RectangleCollider(int x, int y, int width, int height)
    {
        Boundary = new Rectangle(x, y, width, height);
    }

    public static bool IsCollided(RectangleCollider r1, RectangleCollider r2)
    {
        return r1.Boundary.Intersects(r2.Boundary);
    }
}