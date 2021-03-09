using System;
using UnityEngine;

public class Line
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

    public float GetXAt(float y)
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

    public Vector2? GetIntersectionWith(Line line)
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

    public void SetIsLegal(bool isLegal)
    {
        this.isLegal = isLegal;
    }

    public bool IsLegal()
    {
        return this.isLegal;
    }

}
