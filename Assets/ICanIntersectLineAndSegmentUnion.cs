using UnityEngine;

public interface ICanIntersectLineAndSegmentUnion : ICanIntersectLineAndSegment
{
    Vector2? GetIntersectionWithLineAndSegmentUnion(ILineAndSegmentUnion lineAndSegmentUnion);
}