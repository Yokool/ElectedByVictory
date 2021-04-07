using UnityEngine;

public interface ICanIntersectLineSegment
{
    Vector2? GetIntersectionWithLineSegment(LineSegment lineSegment);
}
