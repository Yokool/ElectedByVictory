using ElectedByVictory.WorldCreation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TriangleUtilities
{
    public static UnsafeTriangle[] FilterTrianglesByExcircle(UnsafeTriangle[] triangles)
    {

        UnsafeTriangle[] trianglesCopy = triangles.ToArray();

        for (int i = 0; i < trianglesCopy.Length; ++i)
        {
            UnsafeTriangle checkingTriangle = trianglesCopy[i];

            if (checkingTriangle == null)
            {
                continue;
            }

            Circle checkingTriangleExcircle = checkingTriangle.GetExcircle();

            for (int j = 0; j < trianglesCopy.Length; ++j)
            {

                // Do not check itself against itself.
                if (i == j)
                {
                    continue;
                }

                UnsafeTriangle checkedTriangle = trianglesCopy[j];

                if (checkedTriangle == null)
                {
                    continue;
                }

                Vector2 checkedTriangleCentroid = checkedTriangle.GetCentroid();

                bool checkingContainsChecked = checkingTriangleExcircle.ContainsPoint(checkedTriangleCentroid);

                if (checkingContainsChecked)
                {
                    trianglesCopy[j] = null;
                }

            }

        }

        return trianglesCopy.Where((triangle) => { return (triangle != null); }).ToArray();
    }
}
