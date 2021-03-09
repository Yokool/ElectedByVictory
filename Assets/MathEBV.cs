using UnityEngine;

public static class MathEBV
{
    public static bool FloatEquals(float f1, float f2, float epsilon = 0.01f)
    {
        return System.Math.Abs(f2 - f1) < epsilon;
    }

    public static bool IsFloatZero(float f)
    {
        return FloatEquals(f, 0.0f);
    }

    public static bool PointsEqual(Vector2 p1, Vector2 p2, float epsilon = 0.01f)
    {
        return MathEBV.FloatEquals(p1.x, p2.x, epsilon) && MathEBV.FloatEquals(p1.y, p2.y, epsilon);
    }

}