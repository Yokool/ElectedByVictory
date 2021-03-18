using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformationUtilities
{
    /// <summary>
    /// Transforms positions from local space to world space.
    /// </summary>
    public static Vector3[] TransformPoints(this Transform transform, Vector3[] points)
    {
        Vector3[] transformedPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; ++i)
        {
            Vector3 point = points[i];
            transformedPoints[i] = transform.TransformPoint(point);
        }
        return transformedPoints;
    }
    /// <summary>
    /// Transforms positions from world space to local space.
    /// </summary>
    public static Vector3[] InverseTransformPoints(this Transform transform, Vector3[] points)
    {
        Vector3[] transformedPoints = new Vector3[points.Length];
        for (int i = 0; i < points.Length; ++i)
        {
            Vector3 point = points[i];
            transformedPoints[i] = transform.InverseTransformPoint(point);
        }
        return transformedPoints;
    }

}
