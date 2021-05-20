using UnityEngine;

public interface ICutOffLineRaySegmentUnion : ICutOffLine, ICutOffLineSegment, ICutOffRay
{
    ILineRaySegmentUnion CutOff(Vector2 planePoint, ILineRaySegmentUnion lineRaySegmentUnion);
}
