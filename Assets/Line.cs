using System;
using UnityEngine;

public sealed class Line : IHasSlopeAndYIntercept, ILineAndSegmentUnion, IGetDeepPerpendicularLine
{
    /// <summary>
    /// Contains the slope of the line,
    /// If null then the line is vertical and it's position
    /// is set by the y intercept.
    /// </summary>
    private float? m;

    private float b;

    private bool isLegal = false;

    public override string ToString()
    {
        float? slope = GetSlope();
        string slopeString = slope.HasValue ? Convert.ToString(slope.Value) : "undefined";

        string returnValue = $"LINE: m = {{{slopeString}}} b = {{{GetYIntercept()}}}";
        
        return returnValue;
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

        if (!slope.HasValue)
        {
            return null;
        }

        return slope.Value * x + GetYIntercept();
    }

    public Line GetShallowPerpendicularLine()
    {
        return new Line(GetPerpendicularSlope(), 0f);
    }

    public float? GetXAt(float y)
    {
        float? slope = GetSlope();

        if (!slope.HasValue)
        {
            // b equals the x position of the line in this case
            return GetYIntercept();
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
        // If it's vertical then m = undefined and we can't get its value
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

    public void SetSlope(float x1, float y1, float x2, float y2)
    {
        bool isLegal = !(MathEBV.FloatEquals(x1, x2) && MathEBV.FloatEquals(y1, y2));
        SetIsLegal(isLegal);

        if (!isLegal)
        {
            Debug.LogError($"Tried to set a slope of the line: {this} - out of two points a, b where a = b; a = {{x: {x1} y: {y1}}}; b = {{x: {x2} y. {y2}}}. This is not possible, we need two non-equal points to construct a line.");
            return;
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

    public void SetSlope(float? m)
    {
        this.m = m;
        SetIsLegal(true);
    }

    public void SetYIntercept(float b)
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
            return intersectionPoint.Value;
        }

        /*
        if(!m1.HasValue || !m2.HasValue)
        {
            Debug.LogWarning("How did we get here?");
            return null;
        }
        */


        float denominator = m1.Value - m2.Value;

        // Both lines are parallel (no intersection unless they are the exact same lines [their equations equal])
        // Even if their equations equal, then we can't get a sensible point since they overlap eachother
        // Both lines could also be horizontal, but the same applies
        if(MathEBV.FloatEquals(denominator, 0f))
        {
            return null;
        }

        float x = (b2 - b1) / (denominator);
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

    public void SetIsLegal(bool isLegal)
    {
        this.isLegal = isLegal;
    }

    public bool IsLegal()
    {
        return this.isLegal;
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
        return null;

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

    public Vector2? GetIntersectionWithLineAndSegmentUnion(ILineAndSegmentUnion lineAndSegmentUnion)
    {
        return lineAndSegmentUnion.GetIntersectionWithLine(this);
    }
}
