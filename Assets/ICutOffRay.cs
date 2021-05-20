using UnityEngine;

public interface ICutOffRay : IPointPlaneAssignment
{
    ILineRaySegmentUnion CutOffRay(Vector2 planePoint, LineRay ray);
}
