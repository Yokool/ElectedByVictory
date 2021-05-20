using UnityEngine;
using System;

public static class PointUtilities
{
    public static bool AreThreePointsInLine(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Line p1ToP2 = new Line(p1, p2);

        return p1ToP2.ContainsPoint(p3);
    }


    public static float PointDistance(Vector2 p1, Vector2 p2)
    {
        float xDiff = Math.Abs(p2.x - p1.x);
        float yDiff = Math.Abs(p2.y - p1.y);

        return (float)Math.Sqrt(Math.Pow(xDiff, 2.0) + Math.Pow(yDiff, 2.0));
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

    public static Vector2 ClampPointToInterval(Vector2 intervalStart, Vector2 intervalEnd, Vector2 point)
    {
        float intervalXStart = Math.Min(intervalStart.x, intervalEnd.x);
        float intervalXEnd = Math.Max(intervalStart.x, intervalEnd.x);

        float intervalYStart = Math.Min(intervalStart.y, intervalEnd.y);
        float intervalYEnd = Math.Max(intervalStart.y, intervalEnd.y);

        float clampedX = Mathf.Clamp(point.x, intervalXStart, intervalXEnd);
        float clampedY = Mathf.Clamp(point.y, intervalYStart, intervalYEnd);

        return new Vector2(clampedX, clampedY);
    }

    public static Vector2 AxisSymmetry(Vector2 axisPoint, Vector2 point)
    {
        float dX = (axisPoint.x - point.x);
        float dY = (axisPoint.y - point.y);

        float symmetryX = (point.x + (dX * 2));
        float symmetryY = (point.y + (dY * 2));

        return new Vector2(symmetryX, symmetryY);
    }

}
