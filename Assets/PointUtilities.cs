using UnityEngine;
using System;
using ElectedByVictory.WorldCreation;
using System.Linq;

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

    public static Vector3[] Cast2DVerticesTo3D(Vector2[] _2DVertices)
    {
        return Cast2DVerticesTo3DAndSetZ(_2DVertices, 0.0f);
    }

    public static Vector3[] Cast2DVerticesTo3DAndSetZ(Vector2[] _2DVertices, float z)
    {
        Vector3[] _3DVertices = new Vector3[_2DVertices.Length];
        for (int i = 0; i < _2DVertices.Length; ++i)
        {
            Vector2 _2DVertex = _2DVertices[i];
            _3DVertices[i] = new Vector3(_2DVertex.x, _2DVertex.y, z);
        }
        return _3DVertices;
    }

    public static Vector2[] SortPointsByAngleToPointToCopy(Vector2 anglePoint, Vector2[] points)
    {
        Vector2[] points_copy = points.ToArray();

        double[] angles = new double[points_copy.Length];
        

        for(int i = 0; i < points_copy.Length; ++i)
        {
            Vector2 point = points_copy[i];
            Vector2 differenceVector = (point - anglePoint).normalized;

            angles[i] = PointMath.PointAtan2(differenceVector);
        }


        for (int i = 0; i < points_copy.Length; ++i)
        {
            int smallerAngleIndex = i;
            double smallestAngle = angles[i];

            for(int j = (i + 1); j < points_copy.Length; ++j)
            {
                if(angles[j] < smallestAngle)
                {
                    smallestAngle = angles[j];
                    smallerAngleIndex = j;
                }
            }

            if(smallerAngleIndex == i)
            {
                continue;
            }

            Vector2 bufferPoint = points_copy[i];
            double bufferAngle = angles[i];


            points_copy[i] = points_copy[smallerAngleIndex];
            angles[i] = angles[smallerAngleIndex];

            points_copy[smallerAngleIndex] = bufferPoint;
            angles[smallerAngleIndex] = bufferAngle;
        }

        return points_copy;
    }


    public static bool ArrayContainsPoint(Vector2[] points, Vector2 checkedPoint)
    {
        for(int i = 0; i < points.Length; ++i)
        {
            Vector2 arrayPoint = points[i];
            if(PointMath.PointEquals(arrayPoint, checkedPoint))
            {
                return true;
            }
        }

        return false;
    }

}
