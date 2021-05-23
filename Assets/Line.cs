using ElectedByVictory.WorldCreation;
using System;
using UnityEngine;

public sealed class Line : IHasSlopeAndYIntercept, ILineRaySegmentUnion, IGetDeepPerpendicularLine
{
    /// <summary>
    /// Contains the slope of the line,
    /// If null then the line is vertical and it's position
    /// is set by the y intercept.
    /// </summary>
    private float? m;

    private float b;

    public override string ToString()
    {
        float? slope = GetSlope();
        string slopeString = slope.HasValue ? Convert.ToString(slope.Value) : "undefined";

        string returnValue = $"LINE: m = {{{slopeString}}} b = {{{GetYIntercept()}}}";
        
        return returnValue;
    }

    public Line(Vector2 p1, Vector2 p2) : this(p1.x, p1.y, p2.x, p2.y)
    {

    }

    public Line(float x1, float y1, float x2, float y2)
    {
        SetLineValuesFromPoints(x1, y1, x2, y2);
    }

    public Line(float? m, float b)
    {
        SetLineValues(m, b);
    }

    /// <summary>
    /// Sets the y intercept of this line without changing the slope of this line
    /// to assure that the line will travel through a point (x, y)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetTravelThrough(float x, float y)
    {

        float? slope = GetSlope();
        if (!slope.HasValue)
        {
            SetYIntercept(x);
            return;
        }

        // b = y - mx
        float b = y - GetSlope().Value * x;
        SetYIntercept(b);
    }

    public void SetTravelThroughPoint(Vector2 point)
    {
        SetTravelThrough(point.x, point.y);
    }

    public float? GetYAt(float x)
    {
        // y = mx + b
        float? slope = GetSlope();

        // Either return no point or all the Ys, which doesn't make sense so return null. 
        if (IsVertical())
        {
            return null;
        }

        return slope.Value * x + GetYIntercept();
    }

    private Line GetShallowPerpendicularLine()
    {
        return new Line(GetPerpendicularSlope(), 0f);
    }

    public float? GetXAt(float y)
    {
        float? slope = GetSlope();

        // For a vertical line, all the values Y are on the position of the line, also known as
        // you will find any Y at the x position of the vertical line.
        if (IsVertical())
        {
            // b equals the x position of the line in this case
            return GetYIntercept();
        }

        // If it is horizontal, then you will find that the line has only a single value of y. Therefore
        // you will either find the y you want anywhere on the line or you won't find it at all. Therefore return null.
        // If y = 5 and your param is 5, then there is no single x coordinate associated with 5
        // there is an infinite number of them.
        // If y = 5 and your param is 6, then there doesn't exist such a point on the line and you will get nothing.
        // Therefore in both situations return null.
        if(IsHorizontal())
        {
            return null;
        }

        // x = (y - b) / m
        return (y - GetYIntercept()) / slope.Value;
    }

    /*
    /// <summary>
    /// Returns a line that is perpendicular to this one
    /// </summary>
    /// <returns></returns>
    public Line GetPerpendicular()
    {
        return new Line(GetPerpendicularSlope(), GetYIntercept());
    }
    */

    public float? GetPerpendicularSlope()
    {
        float? slope = GetSlope();

        if (IsVertical())
        {
            return 0f;
        }

        if (IsHorizontal())
        {
            return null;
        }

        return -1f / slope.Value;
    }

    public bool IsHorizontal()
    {
        // If it's vertical then m = undefined and we can't get its .Value
        if (IsVertical())
        {
            return false;
        }

        return MathEBV.IsFloatZero(GetSlope().Value);
    }

    public bool IsVertical()
    {
        return !GetSlope().HasValue;
    }


    public void SetYIntercept(float x, float y)
    {
        // y - mx = b
        float b = LineUtilities.GetYInterceptFromPoint(GetSlope(), x, y);
        SetYIntercept(b);
    }

    public float? GetSlope()
    {
        return this.m;
    }

    public float GetYIntercept()
    {
        return this.b;
    }

    public Vector2? GetIntersectionWithRay(LineRay ray)
    {
        return ray.GetIntersectionWithLine(this);
    }

    public void SetSlope(float x1, float y1, float x2, float y2)
    {
        bool isIllegal = (MathEBV.FloatEquals(x1, x2) && MathEBV.FloatEquals(y1, y2));

        if (isIllegal)
        {
            throw ExceptionUtilities.ILLEGAL_LINE_EXCEPTION(this, x1, y1, x2, y2);
        }

        // TODO: DENOMINATOR = 0
        float numerator = (y2 - y1);
        float denominator = (x2 - x1);

        if (MathEBV.FloatEquals(denominator, 0f))
        {
            SetSlope(null);
            return;
        }

        SetSlope(numerator / denominator);
    }

    public void SetLineValues(float? m, float b)
    {
        SetSlope(m);
        SetYIntercept(b);
    }

    public void SetLineValuesFromPoints(float x1, float y1, float x2, float y2)
    {
        SetSlope(x1, y1, x2, y2);
        SetYIntercept(x1, y1);
    }

    private void SetSlope(float? m)
    {
        this.m = m;
    }

    private void SetYIntercept(float b)
    {
        this.b = b;
    }

    public Vector2? GetIntersectionWithLine(Line line)
    {

        // y1 = m1x + b1
        // y2 = m2x + b2
        // y1 = y2
        // m1x + b1 = m2x + b2
        // m1x - m2x = b2 - b1
        // x * (m1 - m2) = b2 - b1
        // x = (b2 - b1)/(m1 - m2)

        float? m1 = GetSlope();
        float? m2 = line.GetSlope();

        float b1 = GetYIntercept();
        float b2 = line.GetYIntercept();

        if (LineUtilities.TrySpecialLineIntersection(m1, b1, m2, b2, out Vector2? intersectionPoint))
        {
            return intersectionPoint;
        }

        /*
        if(!m1.HasValue || !m2.HasValue)
        {
            Debug.LogWarning("How did we get here?");
            return null;
        }
        */


        // Both lines are parallel (no intersection unless they are the exact same lines [their equations equal])
        // Even if their equations equal, then we can't get a sensible point since they overlap eachother
        // Both lines could also be horizontal, but the same applies
        if(MathEBV.FloatEquals(m1.Value, m2.Value))
        {
            return null;
        }

        float x = (b2 - b1) / (m1.Value - m2.Value);
        float y = (m1.Value * x) + b1;
        /*
        Debug.Log($"m1: {m1} b1: {b1}");
        Debug.Log($"m2: {m2} b2: {b2}");
        Debug.Log($"x: {x} y: {y}");
        */
        return new Vector2(x, y);

    }

    public Vector2? GetIntersectionWithLineSegment(LineSegment lineSegment)
    {
        // We can easily get the intersection with this line, through the reference to the line segment.
        // It doesn't matter if we call lineSegment.GetIntersectionWith(this) or this.GetIntersectionWith(lineSegment)
        // but the implementation is hidden away in the line segment class.
        Vector2? intersection = lineSegment.GetIntersectionWithLine(this);
        return intersection;
    }

    private static Exception GetLinePlaneAssigment_RETURN_EXCEPTION()
    {
        return new Exception($"{nameof(GetLinePlaneAssignment)} exception.");
    }

    public HalfPlane? GetLinePlaneAssignment(Vector2 point)
    {
        // For vertical lines, the plane assignment is determined based on the x coordinate of the point
        // If the point is left of the line then Plane.BOTTOM
        // If the point is on the right of the line then Plane.TOP
        if(IsVertical())
        {
            // For a vertical line, we'll be able to get the x point always.
            // We couldn't get an X point if the line was horizontal, but that's not an issue here.
            float xAtPointY = GetXAt(point.y).Value;

            if (MathEBV.FloatEquals(point.x, xAtPointY))
            {
                return null;
            }

            if (point.x < xAtPointY)
            {
                return HalfPlane.BOTTOM;
            }
            else if(point.x > xAtPointY)
            {
                return HalfPlane.TOP;
            }

            
        }

        
        

        // Since we've checked for if the line is vertical, we can just get the
        // value immediately. For a non-vertical line, we'll always be able to get the y point anywhere.
        float yAtPointX = GetYAt(point.x).Value;

        if (MathEBV.FloatEquals(point.y, yAtPointX))
        {
            return null;
        }

        if (point.y < yAtPointX)
        {
            return HalfPlane.BOTTOM;
        }
        else if(point.y > yAtPointX)
        {
            return HalfPlane.TOP;
        }

        Debug.LogError("This statement should not be reached.");
        throw GetLinePlaneAssigment_RETURN_EXCEPTION();

    }
    public Line GetDeepPerpendicularLinePoint(Vector2 point)
    {
        return GetDeepPerpendicularLine(point.x, point.y);
    }

    public Line GetDeepPerpendicularLine(float x, float y)
    {
        Line deepPerpendicularLine = GetShallowPerpendicularLine();
        deepPerpendicularLine.SetTravelThrough(x, y);
        return deepPerpendicularLine;
    }

    public Vector2? GetIntersectionWithLineAndSegmentUnion(ILineRaySegmentUnion lineAndSegmentUnion)
    {
        return lineAndSegmentUnion.GetIntersectionWithLine(this);
    }

    public bool ContainsPoint(Vector2 point)
    {

        if(IsVertical())
        {
            return MathEBV.FloatEquals(point.x, GetYIntercept());
        }
        else if(IsHorizontal())
        {
            return MathEBV.FloatEquals(point.y, GetYIntercept());
        }

        float yAtPoint = GetYAt(point.x).Value;

        return MathEBV.FloatEquals(point.y, yAtPoint);

    }


    public bool ThisLineEquals(Line line)
    {
        // From y = mx + b
        // y intercept = b
        // if x = UNDEFINED
        // y intercept => x = y_intercept
        // therefore for two horizontal lines are equal if their y intercepts are equal
        // two vertical lines are equal if their x coordinate is equal, therefore if their y intercepts are equal
        // and two normal lines still have to have the same y intercept
        float thisYIntercept = GetYIntercept();
        float lineYIntercept = GetYIntercept();

        if(!MathEBV.FloatEquals(thisYIntercept, lineYIntercept))
        {
            return false;
        }


        float? thisSlope = GetSlope();
        float? lineSlope = line.GetSlope();

        // Check for
        // UNDEFINED = UNDEFINED (both are vertical, their x coordinate was checked above)
        // Check for
        // 0f = 0f (both are horizontal, special case of m1 = m2, their y coordinate was checked above)
        // Check for
        // m1 = m2 (normal lines, y intercept was checked above)
        return MathEBV.NullableFloatEquals(thisSlope, lineSlope);
    }

    public Vector2 FindNearestPointTo(Vector2 point)
    {
        float? getXAtY = GetXAt(point.y);
        float? getYAtX = GetYAt(point.x);

        if(IsVertical())
        {
            return new Vector2(getXAtY.Value, point.y);
        }

        if(IsHorizontal())
        {
            return new Vector2(point.x, getYAtX.Value);
        }

        float distanceHorizontal = Math.Abs(point.x - getXAtY.Value);
        float distanceVertical = Math.Abs(point.y - getYAtX.Value);

        if(distanceHorizontal < distanceVertical)
        {
            return new Vector2(getXAtY.Value, point.y);
        }
        else
        {
            return new Vector2(point.x, getYAtX.Value);
        }

    }

    public ILineRaySegmentUnion CutOff(Vector2 planePoint, ILineRaySegmentUnion lineRaySegmentUnion)
    {
        return lineRaySegmentUnion.BeCutOffBy(planePoint, this);
    }

    public Line GetEqualLine()
    {
        return new Line(GetSlope(), GetYIntercept());
    }

    public Line GetDeepParallelLine(Vector2 point)
    {
        Line equalLine = GetEqualLine();
        equalLine.SetTravelThroughPoint(point);
        return equalLine;
    }

    public ILineRaySegmentUnion CutOffLine(Vector2 planePoint, Line line)
    {
        // Don't cut off equal lines
        if(ThisLineEquals(line))
        {
            return line;
        }


        Vector2? intersection = GetIntersectionWithLine(line);

        // Parallel lines, then the whole of the line is either in or outside the halfplane
        if (!intersection.HasValue)
        {
            /*
            float? lineX = line.GetXAt(planePoint.y);
            float? lineY = line.GetYAt(planePoint.x);

            float linePointX = (line.IsVertical()) ? (lineX.Value) : (planePoint.x);
            float linePointY = (line.IsVertical()) ? (planePoint.y) : (lineY.Value);
            
            Vector2 linePoint = new Vector2(linePointX, linePointY);
            */

            Vector2 linePoint = line.FindNearestPointTo(planePoint);

            // We either keep the whole line, or we get rid of it.
            bool returnLine = IsPointInHalfPlane(planePoint, linePoint);

            return returnLine ? line : null;
        }

        //The parallelLine will be in its entirety inside the half plane in which the
        //planePoint is. Therefore any intersection it makes will also be inside the plane.
        Line parallelLine = GetDeepParallelLine(planePoint);
        
        // Parameter line is not parallel with our line, that was checked by the intersection with this line earlier. 
        Vector2 insidePlaneIntersection = parallelLine.GetIntersectionWithLine(line).Value;

        return new LineRay(intersection.Value, insidePlaneIntersection);
    }

    public ILineRaySegmentUnion CutOffLineSegment(Vector2 planePoint, LineSegment lineSegment)
    {
        Vector2? intersection = GetIntersectionWithLineSegment(lineSegment);

        Vector2 endpoint1 = lineSegment.GetEndPoint1();
        Vector2 endpoint2 = lineSegment.GetEndPoint2();

        
        bool endPoint1_HalfPlane = IsPointInHalfPlane(planePoint, endpoint1);
        bool endPoint2_HalfPlane = IsPointInHalfPlane(planePoint, endpoint2);
        bool isLineSegmentEntiretyInsideHalfPlane = (endPoint1_HalfPlane && endPoint2_HalfPlane);


        if (!intersection.HasValue)
        {
            return (isLineSegmentEntiretyInsideHalfPlane) ? (lineSegment) : (null);
        }

        Vector2 intersectionValue = intersection.Value;

        Vector2? endpoint = null;



        bool endPointOnSplitLine = (PointMath.PointEquals(endpoint1, intersectionValue) || PointMath.PointEquals(endpoint2, intersectionValue));

        if (endPointOnSplitLine)
        {
            return (isLineSegmentEntiretyInsideHalfPlane) ? (lineSegment) : (null);
        }
        else if (endPoint1_HalfPlane)
        {
            endpoint = endpoint1;
        }
        else if(endPoint2_HalfPlane)
        {
            endpoint = endpoint2;
        }

        return new LineSegment(intersectionValue, endpoint.Value);
        
        
        
    }

    public ILineRaySegmentUnion CutOffRay(Vector2 planePoint, LineRay ray)
    {
        Vector2 rayOrigin = ray.GetOrigin();
        Vector2? intersection = GetIntersectionWithRay(ray);
        bool isOriginInHalfPlane = IsPointInHalfPlane(planePoint, rayOrigin);

        if (!intersection.HasValue)
        {;
            bool isPointInHalfPlane = isOriginInHalfPlane;
            return (isPointInHalfPlane) ? (ray) : (null);
        }


        if (PointMath.PointEquals(rayOrigin, intersection.Value))
        {
            Vector2 rayInnerPoint = ray.GetInnerPoint();

            bool isRayInHalfPlane = IsPointInHalfPlane(planePoint, rayInnerPoint);

            return (isRayInHalfPlane) ? (ray) : (null);
        }
        
        // If the origin is inside the plane and we have intersected the ray, then the ray is
        // trying to exit the plane, so cut it off into a segment.
        if(isOriginInHalfPlane)
        {
            return new LineSegment(intersection.Value, rayOrigin);
        }
        // The origin is outside the plane and we have intersected, thus it is trying to get into the
        // half plane, so we keep the ray but move its origin.
        else
        {
            Vector2 symmetryHalfPlaneInnerPoint = PointUtilities.AxisSymmetry(intersection.Value, rayOrigin);
            return new LineRay(intersection.Value, symmetryHalfPlaneInnerPoint);
        }
    }

    public bool IsPointInHalfPlane(Vector2 planePoint, Vector2 point)
    {
        HalfPlane? planePointHalfPlane = GetLinePlaneAssignment(planePoint);

        // The plane point is in both planes, therefore all other points will be in both planes.
        if(!planePointHalfPlane.HasValue)
        {
            return true;
        }

        return IsPointInHalfPlane(planePointHalfPlane.Value, point);
    }

    public bool IsPointInHalfPlane(HalfPlane plane, Vector2 point)
    {
        HalfPlane? pointHalfPlane = GetLinePlaneAssignment(point);

        // The plane is in both planes, therefore it is in the parameter plane.
        if(!pointHalfPlane.HasValue)
        {
            return true;
        }

        return plane.Equals(pointHalfPlane);
    }

    public ILineRaySegmentUnion BeCutOffBy(Vector2 planePoint, Line planeSeperationLine)
    {
        return planeSeperationLine.CutOffLine(planePoint, this);
    }
}
