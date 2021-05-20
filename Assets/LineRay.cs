using UnityEngine;

public class LineRay : ILineRaySegmentUnion, IHasSlope
{

    private Vector2 origin;
    private Vector2 innerPoint;

    private Line underlyingLine;

    public LineRay(Vector2 origin, Vector2 innerPoint)
    {
        SetOrigin(origin);
        SetInnerPoint(innerPoint);
        UpdateUnderlyingLine();
    }

    private Line GetUnderlyingLine()
    {
        return this.underlyingLine;
    }

    private void SetUnderlyingLine(Line line)
    {
        this.underlyingLine = line;
    }

    private void UpdateUnderlyingLine()
    {
        Vector2 origin = GetOrigin();
        Vector2 innerPoint = GetInnerPoint();
        Line underlyingLine = new Line(origin, innerPoint);
        SetUnderlyingLine(underlyingLine);
    }

    public void SetOrigin(Vector2 origin)
    {
        this.origin = origin;
    }

    public Vector2 GetOrigin()
    {
        return this.origin;
    }

    public void SetInnerPoint(Vector2 innerPoint)
    {
        this.innerPoint = innerPoint;
    }

    public Vector2 GetInnerPoint()
    {
        return this.innerPoint;
    }

    

    public bool ContainsPoint(Vector2 point)
    {
        if(!LineUtilities.IsPointInRayInterval(this, point))
        {
            return false;
        }

        return underlyingLine.ContainsPoint(point);
    }

    public Vector2? GetIntersectionWithLine(Line line)
    {
        Vector2? underlyingIntersection = GetUnderlyingLine().GetIntersectionWithLine(line);


        if(!underlyingIntersection.HasValue)
        {
            return null;
        }

        if(ContainsPoint(underlyingIntersection.Value))
        {
            return underlyingIntersection.Value;
        }
        else
        {
            return null;
        }

    }

    public Vector2? GetIntersectionWithLineAndSegmentUnion(ILineRaySegmentUnion lineAndSegmentUnion)
    {
        return lineAndSegmentUnion.GetIntersectionWithRay(this);
    }

    public Vector2? GetIntersectionWithLineSegment(LineSegment lineSegment)
    {
        Vector2? intersection = GetUnderlyingLine().GetIntersectionWithLineSegment(lineSegment);

        if(!intersection.HasValue && !ContainsPoint(intersection.Value))
        {
            return null;
        }

        return intersection.Value;
    }

    public HalfPlane? GetLinePlaneAssignment(Vector2 point)
    {
        return GetUnderlyingLine().GetLinePlaneAssignment(point);
    }

    public float? GetXAt(float y)
    {
        if (!LineUtilities.IsYInRayInterval(y, this))
        {
            return null;
        }

        float? xAt = GetUnderlyingLine().GetXAt(y);
        return xAt;

    }

    public float? GetYAt(float x)
    {
        if(!LineUtilities.IsXInRayInterval(x, this))
        {
            return null;
        }

        float? yAt = GetUnderlyingLine().GetYAt(x);
        return yAt;
    }


    public Vector2? GetIntersectionWithRay(LineRay ray)
    {
        Vector2? intersection = GetUnderlyingLine().GetIntersectionWithRay(ray);

        if(!intersection.HasValue && !ContainsPoint(intersection.Value))
        {
            return null;
        }

        return intersection;
        
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

    public Vector2 FindNearestPointTo(Vector2 point)
    {
        Vector2 nearestUnderlyingPoint = GetUnderlyingLine().FindNearestPointTo(point);

        (Vector2, Vector2) intervalPoints = LineUtilities.GetLineRayPointInterval(this);
        Vector2 intervalStart = intervalPoints.Item1;
        Vector2 intervalEnd = intervalPoints.Item2;

        return PointUtilities.ClampPointToInterval(nearestUnderlyingPoint, intervalStart, intervalEnd);
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

    public ILineRaySegmentUnion BeCutOffBy(Vector2 planePoint, Line planeLine)
    {
        return planeLine.CutOffRay(planePoint, this);
    }
}
