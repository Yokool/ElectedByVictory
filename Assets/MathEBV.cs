using UnityEngine;
using System;

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

    public static bool PointsEqualNullable(Vector2? p1, Vector2? p2, float epsilon = 0.01f)
    {
        if(p1.HasValue && p2.HasValue)
        {
            return PointsEqual(p1.Value, p2.Value, epsilon);
        }
        else
        {
            bool bothHaveNoValue = !p1.HasValue && !p2.HasValue;
            // If only one of them is nullable and the other one isn't return false, otherwise return true
            return bothHaveNoValue;
        }
    }

    public static bool IsValueInClosedInterval(float value, float min, float max)
    {
        return ((value >= min) && (value <= max));
    }

    public static float PointDistance(float x1, float y1, float x2, float y2)
    {
        float xDiff = (x2 - x1);
        float yDiff = (y2 - y1);

        return (float)Math.Sqrt(Math.Pow(xDiff, 2.0) + Math.Pow(yDiff, 2.0));
    }

    public static float PointDistance(Vector2 p1, Vector2 p2)
    {
        return MathEBV.PointDistance(p1.x, p1.y, p2.x, p2.y);
    }

}