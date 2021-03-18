using UnityEngine;
using SystemMath = System.Math;

public static class PointUtilities
{
    public static float PointDistance(Vector2 p1, Vector2 p2)
    {
        float xDiff = SystemMath.Abs(p2.x - p1.x);
        float yDiff = SystemMath.Abs(p2.y - p1.y);

        return (float)SystemMath.Sqrt(SystemMath.Pow(xDiff, 2.0) + SystemMath.Pow(yDiff, 2.0));
    }

    public static Vector2 ClampPointToLine(Vector2 origin, Vector2 pointToClamp, Line clampingLine)
    {
        float o_x = origin.x;
        float o_y = origin.y;

        float p_x = pointToClamp.x;
        float p_y = pointToClamp.y;

        LineSegment originToPoint = new LineSegment(o_x, o_y, p_x, p_y);


        Vector2? intersection = originToPoint.GetIntersectionWithLine(clampingLine);
        /*
        Debug.Log("?????: " + intersection);

        Debug.Log($"or: {origin}");
        Debug.Log($"ptc: {pointToClamp}");
        Debug.Log($"inter: {intersection}");
        */
        // If thel lines don't intersect, don't clamp the point
        if (!intersection.HasValue)
        {
            return pointToClamp;
        }
        /*
        float distanceToPoint = PointUtilities.PointDistance(origin, pointToClamp);

        float distanceToClampedPoint = PointUtilities.PointDistance(origin, intersection.Value);

        if (distanceToPoint < distanceToClampedPoint)
        {
            return pointToClamp;
        }
        else
        {
            return intersection.Value;
        }
        */
        return intersection.Value;
    }

    public static bool IsPointInClosedInterval(Vector2 point, float xMin, float xMax, float yMin, float yMax)
    {
        return (MathEBV.IsValueInClosedInterval(point.x, xMin, xMax) && MathEBV.IsValueInClosedInterval(point.y, yMin, yMax));
    }

}
