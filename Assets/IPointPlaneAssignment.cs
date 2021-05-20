using UnityEngine;

public interface IPointPlaneAssignment
{
    HalfPlane? GetLinePlaneAssignment(Vector2 point);

    bool IsPointInHalfPlane(Vector2 planePoint, Vector2 point);
    bool IsPointInHalfPlane(HalfPlane plane, Vector2 point);

}
