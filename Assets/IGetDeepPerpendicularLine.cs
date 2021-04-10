using UnityEngine;

public interface IGetDeepPerpendicularLine
{
    Line GetDeepPerpendicularLinePoint(Vector2 point);
    Line GetDeepPerpendicularLine(float x, float y);
}
