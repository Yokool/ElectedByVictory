using System;
using UnityEngine;


namespace ElectedByVictory.WorldCreation
{
    public static class PointMath
    {
        public static float AngleBetweenPointsRad(Vector2 p1, Vector2 p2)
        {
            // Transform this into a "positive" triangle. Since the angle will be the same no matter
            // to which side it points.
            float diffX = Math.Abs(p2.x - p1.x);
            float diffY = Math.Abs(p2.y - p1.y);

            float angle = (float)Math.Atan(diffY / diffX);

            return angle;
        }
    }

}
