using System;
using Microsoft.Xna.Framework;

namespace RoguelikeGame.GameModel.Helpers;

public static class Vector2Extensions
{
    public static double GetDistanceTo(this Vector2 firstVector, Vector2 secondVector)
    {
        var deltaVector = firstVector - secondVector;
        
        return Math.Sqrt(deltaVector.X * deltaVector.X + deltaVector.Y * deltaVector.Y);
    }

    public static Vector2 GetDirectionToPosition(this Vector2 initialPosition, Vector2 destination)
    {
        var deltaVector = destination - initialPosition;

        return Vector2.Normalize(deltaVector);
    }
}