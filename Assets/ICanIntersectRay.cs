using UnityEngine;

public interface ICanIntersectRay
{
    Vector2? GetIntersectionWithRay(LineRay ray);
}