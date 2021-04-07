using UnityEngine;

public interface IPointPlaneAssignment
{
    HalfPlane? GetLinePlaneAssignment(Vector2 point);
}
