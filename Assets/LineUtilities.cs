using ElectedByVictory.WorldCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LineUtilities
{
    /// <summary>
    /// Tries to get the intersection point for two lines, when the following condition is met:
    /// L1 = vertical || L2 = horizontal
    /// L2 = horizontal || L1 = vertical
    /// 
    /// As long as m1 = null or m2 = null, this method will handle the scenario.
    /// 
    /// </summary>
    /// <returns>True if this method handled the line intersection, it follows then that the user can use intersectionPoint out variable.</returns>
    public static bool TrySpecialLineIntersection(float? m1, float b1, float? m2, float b2, out Vector2? intersectionPoint)
    {
        intersectionPoint = null;

        bool L1Vertical = !m1.HasValue;
        bool L2Vertical = !m2.HasValue;

        // If both lines are not vertical, then we don't process it inside of this method
        if(!L1Vertical && !L2Vertical)
        {
            return false;
        }

        // Both lines are vertical, thus they are parallel
        // For lines if their x location is different, then they will never
        // meet.
        // They could also overlap each other but with lines we can't get a sensible
        // point.
        if(L1Vertical && L2Vertical)
        {
            return true;
        }

        // L1 = vertical => L2 = normal/horizontal
        if (L1Vertical)
        {
            intersectionPoint = LeftNormalRightVerticalIntersection(m2.Value, b2, b1);
        }
        // L2 = vertical => L1 = normal/horizontal
        else if (L2Vertical)
        {
            intersectionPoint = LeftNormalRightVerticalIntersection(m1.Value, b1, b2);
        }

        if(!intersectionPoint.HasValue)
        {
            throw ExceptionUtilities.CODE_BLOCK_SHOULD_NOT_BE_REACHED_EXCEPTION();
        }

        return true;

    }

    private static Vector2 LeftNormalRightVerticalIntersection(float m1, float b1, float b2)
    {
        float x = b2;
        float y = m1 * b2 + b1;
        return new Vector2(x, y);
    }


    public static float GetYInterceptFromPoint(float? m, float x, float y)
    {
        //x = b
        if (!m.HasValue)
        {
            return x;
        }

        float b = y - m.Value * x;
        return b;
    }
    
    public static (Vector2, Vector2) GetLineRayPointInterval(LineRay lineRay)
    {
        Vector2 intervalStart = lineRay.GetOrigin();

        Vector2 innerPoint = lineRay.GetInnerPoint();

        float xIntervalEnd = GetLineRayIntervalEnd(intervalStart.x, innerPoint.x);
        float yIntervalEnd = GetLineRayIntervalEnd(intervalStart.y, innerPoint.y);

        return (intervalStart, new Vector2(xIntervalEnd, yIntervalEnd));
    }

    public static bool IsXInRayInterval(float x, LineRay ray)
    {
        Vector2 origin = ray.GetOrigin();
        Vector2 innerPoint = ray.GetInnerPoint();

        float originX = origin.x;
        float innerPointX = innerPoint.x;



        return internal_IsInRayInterval(x, originX, innerPointX);
    }

    private static bool internal_IsInRayInterval(float value, float originValue, float innerPointValue)
    {
        float intervalVal = GetLineRayIntervalEnd(originValue, innerPointValue);
        return MathEBV.IsValueInClosedInterval(value, originValue, intervalVal);
    }

    private static float GetLineRayIntervalEnd(float originValue, float innerPointValue)
    {
        return (innerPointValue > originValue) ? (float.MaxValue) : (float.MinValue);
    }

    public static bool IsYInRayInterval(float y, LineRay ray)
    {
        Vector2 origin = ray.GetOrigin();
        Vector2 innerPoint = ray.GetInnerPoint();

        float originY = origin.y;
        float innerPointY = innerPoint.y;

        return internal_IsInRayInterval(y, originY, innerPointY);
    }

    public static bool IsPointInRayInterval(LineRay lineRay, Vector2 point)
    {
        return (IsXInRayInterval(point.x, lineRay) && IsYInRayInterval(point.y, lineRay));
    }

    public static bool IsPointInLineSegmentInterval(LineSegment lineSegment, Vector2 point)
    {
        Vector2 endPoint1 = lineSegment.GetEndPoint1();
        Vector2 endPoint2 = lineSegment.GetEndPoint2();

        LineRay ray1 = new LineRay(endPoint1, endPoint2);
        LineRay ray2 = new LineRay(endPoint2, endPoint1);

        bool ray1Interval = IsPointInRayInterval(ray1, point);
        bool ray2Interval = IsPointInRayInterval(ray2, point);

        return (ray1Interval && ray2Interval);
    }

    public static Vector2[] GetAllIntersections(ILineRaySegmentUnion[] lines)
    {
        List<Vector2> intersections = new List<Vector2>();
        
        for (int i = 0; i < lines.Length; ++i)
        {
            ILineRaySegmentUnion line = lines[i];
            for (int j = 0; j < lines.Length; ++j)
            {

                if(i == j)
                {
                    continue;
                }

                ILineRaySegmentUnion otherLine = lines[j];

                Vector2? intersection = line.GetIntersectionWithLineAndSegmentUnion(otherLine);
                
                if (!intersection.HasValue)
                {
                    continue;
                }

                intersections.Add(intersection.Value);

            }
        }

        /*
        for(int i = 0; i < lines.Length; ++i)
        {

            if(!(lines[i] is LineSegment))
            {
                continue;
            }

            intersections.Add(((LineSegment)lines[i]).GetEndPoint1());
            intersections.Add(((LineSegment)lines[i]).GetEndPoint2());
        }
        */

        /*
        if(lines.Length != intersections.Count)
        {
            for(int i = 0; i < lines.Length; ++i)
            {
                Debug.Log(lines[i]);
            }

            Debug.Log("====");

            for(int i = 0; i < intersections.Count; ++i)
            {
                Debug.Log(intersections[i]);
            }

            throw new Exception("Thug style.");
        }
        */
        return intersections.ToArray();
    }

    public static Vector2[] GetAllUniqueIntersections(ILineRaySegmentUnion[] lines)
    {
        Vector2[] intersections = GetAllIntersections(lines);

        List<Vector2> uniqueIntersections = new List<Vector2>();
        
        for(int i = 0; i < intersections.Length; ++i)
        {
            Vector2 intersection = intersections[i];
            bool alreadyContained = false;

            for(int j = 0; j < uniqueIntersections.Count; ++j)
            {
                Vector2 addedIntersection = uniqueIntersections[j];

                if(PointMath.PointEquals(intersection, addedIntersection))
                {
                    alreadyContained = true;
                    break;
                }
            }

            if(!alreadyContained)
            {
                uniqueIntersections.Add(intersection);
            }

        }

        return uniqueIntersections.ToArray();
    }

}
