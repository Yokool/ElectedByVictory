using ElectedByVictory.WorldCreation;
using UnityEngine;


public class LineSegment : ILineRaySegmentUnion, IHasMidpointAndMidpointLine, IHasSlope
{

    private Line underlyingLine = null;

    private float x1;
    private float y1;

    private float x2;
    private float y2;

    public LineSegment(Vector2 p1, Vector2 p2) : this(p1.x, p1.y, p2.x, p2.y)
    {

    }

    public LineSegment(float x1, float y1, float x2, float y2)
    {
        SetUnderlyingLine(x1, y1, x2, y2);

        SetX1(x1);
        SetY1(y1);

        SetX2(x2);
        SetY2(y2);
    }

    private void SetUnderlyingLine(float x1, float y1, float x2, float y2)
    {
        this.underlyingLine = new Line(x1, y1, x2, y2);
    }

    /// <summary>
    /// Gets a point on the line segment at way percent on the line segment.
    /// 0.5f would get the point at 50% between x1, y1 and x2, y2
    /// </summary>
    /// <param name="way"></param>
    /// <returns></returns>
    public Vector2 GetPointAt(float way)
    {
        //float xDiff = GetX2() - GetX1();
        //float x = GetX1() + (xDiff * way);

        float x = Mathf.Lerp(GetX1(), GetX2(), way);

        float? y = GetYAt(x);

        // If the line is horizontal, then we can't get the y out of the equation
        // and we have to compute it ourselves
        if (!y.HasValue)
        {
            //float yDiff = GetY2() - GetY1();
            //y = GetY1() + (yDiff * way);
            y = Mathf.Lerp(GetY1(), GetY2(), way);
        }

        return new Vector2(x, y.Value);
    }

    public Vector2 GetMidpoint()
    {
        return this.GetPointAt(0.5f);
    }

    public Line GetMidpointLine()
    {
        Vector2 midpoint = GetMidpoint();
        return GetUnderlyingLine().GetDeepPerpendicularLinePoint(midpoint);
    }
    
    /// <summary>
    /// Returns a line segment that is perpendicular to this line segment,
    /// </summary>
    /// <returns></returns>
    public Line GetLinePerpendicularAtWay(float way)
    {
        Vector2 pointAt = GetPointAt(way);

        Line perpendicularLine = new Line(GetPerpendicularSlope(), 0);
        perpendicularLine.SetTravelThrough(pointAt.x, pointAt.y);
        
        return perpendicularLine;
    }

    public Vector2? GetIntersectionWithLine(Line line)
    {
        // Treat this line segment as a line first (through the reference to the underlying line), in order to get the intersection.
        // (underlying line = the line segment represented as an endless line without 2 points defining the segment)
        Vector2? baseLineIntersection = GetUnderlyingLine().GetIntersectionWithLine(line);

        // Then check if the intersection point is on this line (within the bounds of the two points that make the line)
        if (baseLineIntersection.HasValue && IsPointInLineInterval(baseLineIntersection.Value))
        {
            return baseLineIntersection.Value;
        }

        return null;

    }
    public Vector2? GetIntersectionWithLineSegment(LineSegment lineSegment)
    {
        // We treat this line segment as an infinite line
        // and get an intersection with the LINE SEGMENT
        Vector2? baseLineIntersection = GetUnderlyingLine().GetIntersectionWithLineSegment(lineSegment);
        //^That means that we now have an intersection that is guaranteed to be within the bounds/interval of the line segment
        // but not within the bounds of this line.
        // In order to assure that the point is within the bounds of this line, we check it separately
        if (baseLineIntersection.HasValue && IsPointInLineInterval(baseLineIntersection.Value))
        {
            return baseLineIntersection.Value;
        }

        return null;
    }

    public float? GetXAt(float y)
    {
        float? baseX = GetUnderlyingLine().GetXAt(y);

        if (baseX.HasValue && MathEBV.IsValueInClosedInterval(baseX.Value, GetX1(), GetX2()))
        {
            return baseX.Value;
        }

        return null;

    }

    public float? GetYAt(float x)
    {
        float? baseY = GetUnderlyingLine().GetYAt(x);

        if (baseY.HasValue && MathEBV.IsValueInClosedInterval(baseY.Value, GetY1(), GetY2()))
        {
            return baseY.Value;
        }

        return null;
    }

    public bool IsPointInLineInterval(Vector2 point)
    {
        /*
        float smallerX;
        float largerX;

        float smallerY;
        float largerY;

        float x1 = GetX1();
        float x2 = GetX2();
        float y1 = GetY1();
        float y2 = GetY2();

        // Be mind mind that they might be EQUAL, but this should
        // be covered by the setters and shouldn't happen so we might just as well use else.
        if (x1 < x2)
        {
            smallerX = x1;
            largerX = x2;
        }
        else
        {
            smallerX = x2;
            largerX = x1;
        }

        if (y1 < y2)
        {
            smallerY = y1;
            largerY = y2;
        }
        else
        {
            smallerY = y2;
            largerY = y1;
        }

        return PointUtilities.IsPointInClosedInterval(point, smallerX, largerX, smallerY, largerY);
        */

        return LineUtilities.IsPointInLineSegmentInterval(this, point);
    }

    /// <summary>
    /// Documentation (revision 1) - Last Update: 1.4.2021 12:11
    /// The LineSegment plane assignment is done through treating this LineSegment as a Line through the reference
    /// to the underlying values.
    /// </summary>
    public HalfPlane? GetLinePlaneAssignment(Vector2 point)
    {
        return GetUnderlyingLine().GetLinePlaneAssignment(point);
    }
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

    public Vector2 GetEndPoint1()
    {
        return new Vector2(GetX1(), GetY1());
    }

    public Vector2 GetEndPoint2()
    {
        return new Vector2(GetX2(), GetY2());
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
        Line underlyingLine = GetUnderlyingLine();
        underlyingLine.SetSlope(GetX1(), GetY1(), GetX2(), GetY2());
        underlyingLine.SetYIntercept(GetX1(), GetY1());
    }

    private Line GetUnderlyingLine()
    {
        return this.underlyingLine;
    }

    public void SetLineValues(float? m, float b)
    {
        GetUnderlyingLine().SetLineValues(m, b);
    }

    public void SetLineValuesFromPoints(float x1, float y1, float x2, float y2)
    {
        GetUnderlyingLine().SetLineValuesFromPoints(x1, y1, x2, y2);
    }

    public float? GetSlope()
    {
        return GetUnderlyingLine().GetSlope();
    }

    public float? GetPerpendicularSlope()
    {
        return GetUnderlyingLine().GetPerpendicularSlope();
    }

    public bool IsVertical()
    {
        return GetUnderlyingLine().IsVertical();
    }

    public bool IsHorizontal()
    {
        return GetUnderlyingLine().IsHorizontal();
    }

    public Vector2? GetIntersectionWithLineAndSegmentUnion(ILineRaySegmentUnion lineAndSegmentUnion)
    {
        return lineAndSegmentUnion.GetIntersectionWithLineSegment(this);
    }

    public bool ContainsPoint(Vector2 point)
    {
        if(!IsPointInLineInterval(point))
        {
            return false;
        }

        bool underlyingLineCheck = GetUnderlyingLine().ContainsPoint(point);
        return underlyingLineCheck;
    }

    public Vector2? GetIntersectionWithRay(LineRay ray)
    {
        return ray.GetIntersectionWithLineSegment(this);
    }

    public ILineRaySegmentUnion CutOff(Vector2 planePoint, ILineRaySegmentUnion lineRaySegmentUnion)
    {
        return GetUnderlyingLine().CutOff(planePoint, lineRaySegmentUnion);
    }

    public ILineRaySegmentUnion CutOffLine(Vector2 planePoint, Line line)
    {
        return GetUnderlyingLine().CutOffLine(planePoint, line);
    }

    public ILineRaySegmentUnion CutOffLineSegment(Vector2 planePoint, LineSegment lineSegment)
    {
        return GetUnderlyingLine().CutOffLineSegment(planePoint, lineSegment);
    }

    public ILineRaySegmentUnion CutOffRay(Vector2 planePoint, LineRay ray)
    {
        return GetUnderlyingLine().CutOffRay(planePoint, ray);
    }

    public bool IsPointInHalfPlane(Vector2 planePoint, Vector2 point)
    {
        return GetUnderlyingLine().IsPointInHalfPlane(planePoint, point);
    }

    public bool IsPointInHalfPlane(HalfPlane plane, Vector2 point)
    {
        return GetUnderlyingLine().IsPointInHalfPlane(plane, point);
    }

    private LineRay GetRayE1_E2()
    {
        return new LineRay(GetEndPoint1(), GetEndPoint2());
    }

    private LineRay GetRayE2_E1()
    {
        return new LineRay(GetEndPoint2(), GetEndPoint1());
    }

    public Vector2 FindNearestPointTo(Vector2 point)
    {
        LineRay e1ToE2 = GetRayE1_E2();
        LineRay e2ToE1 = GetRayE2_E1();

        Vector2 e1ToE2_ClosePoint = e1ToE2.FindNearestPointTo(point);
        Vector2 e2ToE1_ClosePoint = e2ToE1.FindNearestPointTo(point);

        float e1ToE2_Distance = MathEBV.PointDistance(GetEndPoint1(), e1ToE2_ClosePoint);
        float e2ToE1_Distance = MathEBV.PointDistance(GetEndPoint2(), e2ToE1_ClosePoint);

        if(e1ToE2_Distance < e2ToE1_Distance)
        {
            return e1ToE2_ClosePoint;
        }
        else
        {
            return e2ToE1_ClosePoint;
        }

    }

    public ILineRaySegmentUnion BeCutOffBy(Vector2 planePoint, Line planeLine)
    {
        return planeLine.CutOffLineSegment(planePoint, this);
    }
}
