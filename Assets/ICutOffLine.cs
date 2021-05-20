using UnityEngine;

public interface ICutOffLine : IPointPlaneAssignment
{
    ILineRaySegmentUnion CutOffLine(Vector2 planePoint, Line line);
}
