using UnityEngine;

public class LineSegment : Line
{

    private float x1;
    private float y1;

    private float x2;
    private float y2;

    public LineSegment(float x1, float y1, float x2, float y2) : base(x1, y1, x2, y2)
    {
        SetX1(x1);
        SetY1(y1);

        SetX2(x2);
        SetY2(y2);
    }

    /// <summary>
    /// Gets a point on the line segment at way percent on the line segment.
    /// 0.5f would get the point at 50% between x1, y1 and x2, y2
    /// </summary>
    /// <param name="way"></param>
    /// <returns></returns>
    public Vector2 GetPointAt(float way)
    {
        float xDiff = GetX2() - GetX1();
        float x = GetX1() + (xDiff * way);
        float? y = GetYAt(x);

        // If the line is horizontal, then we can't get the y out of the equation
        // and we have to computer it ourselves
        if (!y.HasValue)
        {
            float yDiff = GetY2() - GetY1();
            y = GetY1() + (yDiff * way);
        }

        return new Vector2(x, y.Value);
    }

    
    /// <summary>
    /// Returns a line segment that is perpendicular to this line segment,
    /// which intersects this line segment exactly halfway.
    /// </summary>
    /// <returns></returns>
    public Line GetLinePerpendicularAtWay(float way)
    {
        Vector2 pointAt = GetPointAt(way);

        //Debug.Log("pointAt: " + pointAt);

        // FINISH THIS
        Line perpendicularLine = new Line(GetPerpendicularSlope(), 0);
        perpendicularLine.SetYIntercept(pointAt.x, pointAt.y);
        /*
        Debug.Log($"L1 - m: {GetSlope()} b: {GetYIntercept()}");
        Debug.Log($"L1 - x1: {GetX1()} y1: {GetY1()}");
        Debug.Log($"L1 - x2: {GetX2()} y2: {GetY2()}");
        Debug.Log($"L2 - m: {perpendicularLine.GetSlope()} b: {perpendicularLine.GetYIntercept()}");
        */
        return perpendicularLine;
    }
    /*
    public LineSegment GetLineSegmentPerpendicularAtWay(float way)
    {
        Line perpendicularLineAtWay = GetLinePerpendicularAtWay(way);

        float x1 = GetX1();
        float y1 = perpendicularLineAtWay.GetYAt(x1);
        float x2 = GetX2();
        float y2 = perpendicularLineAtWay.GetYAt(x2);

        LineSegment lineSegment = new LineSegment(x1, y1, x2, y2);

        return lineSegment;
    }
    */

    public float GetX1()
    {
        return this.x1;
    }

    public float GetX2()
    {
        return this.x2;
    }

    public float GetY1()
    {
        return this.y1;
    }

    public float GetY2()
    {
        return this.y2;
    }

    public void SetX1(float x1)
    {
        this.x1 = x1;
        UpdateLine();
    }

    public void SetX2(float x2)
    {
        this.x2 = x2;
        UpdateLine();
    }

    public void SetY1(float y1)
    {
        this.y1 = y1;
        UpdateLine();
    }

    public void SetY2(float y2)
    {
        this.y2 = y2;
        UpdateLine();
    }

    public void UpdateLine()
    {
        SetSlope(GetX1(), GetY1(), GetX2(), GetY2());
        SetYIntercept(GetX1(), GetY1());
    }
    
}
