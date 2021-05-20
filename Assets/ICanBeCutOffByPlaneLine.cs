using UnityEngine;

public interface ICanBeCutOffByPlaneLine
{
    ILineRaySegmentUnion BeCutOffBy(Vector2 planePoint, Line planeLine);
}