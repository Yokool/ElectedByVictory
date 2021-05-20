using UnityEngine;

public interface ICanIntersectLineRaySegmentUnion : ICanIntersectLine, ICanIntersectLineSegment, ICanIntersectRay
{
    Vector2? GetIntersectionWithLineAndSegmentUnion(ILineRaySegmentUnion lineAndSegmentUnion);
}
