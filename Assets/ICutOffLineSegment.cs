using UnityEngine;

public interface ICutOffLineSegment : IPointPlaneAssignment
{
    ILineRaySegmentUnion CutOffLineSegment(Vector2 planePoint, LineSegment lineSegment);
}
