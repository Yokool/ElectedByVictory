using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TriangleCombinatorics
{

    public static UnsafeTriangle[] PointsIntoTriangles(HashSet<Vector2> pointSet)
    {
        if (pointSet.Count < 3)
        {
            throw new ArgumentException($"There are no triangles to be formed out of less than 3 points.");
        }

        List<UnsafeTriangle> allTriangles = new List<UnsafeTriangle>();

        Vector2[] pointArray = pointSet.ToArray();


        for (int i = 0; i < pointArray.Length; ++i)
        {
            Vector2 point = pointArray[i];
            int elementAfterPointI = (i + 1);

            UnsafeTriangle[] pointTriangles = internalAllTrianglesWhosePointIsAVertex(point, pointArray, elementAfterPointI);

            allTriangles.AddRange(pointTriangles);
        }

        return allTriangles.Where((triangle) => { return (triangle != null); }).ToArray();
    }

    /// <summary>
    /// The parameter all points has to be a set => an array formed out of a HashSet.
    /// <returns></returns>
    private static UnsafeTriangle[] internalAllTrianglesWhosePointIsAVertex(Vector2 A, Vector2[] allPoints, int startIndex)
    {
        if (allPoints.Length < 3)
        {
            throw new ArgumentException("Can't construct triangles out of less than 3 points.");
        }

        // Get our to the first index.
        int indexOfParam = Array.IndexOf(allPoints, A);

        if (indexOfParam == -1)
        {
            throw new ArgumentException($"Parameter point is a member of the {nameof(allPoints)} array.");
        }

        // Computed out of experimentation.
        // Start at our point A, pick the first point after A, which will be B, with the
        // rest of the points we can form (n - 2) triangles whose vertices are A, B, any other C from allPoints.
        // Then from the point i + 2 we can form (n - 3) triangles
        // therefore
        // if pointCount = 5
        // A = 1
        // B = 2
        // C = 3, 4, 5
        // 3 triangles.. then
        // B = 3
        // C = 4, 5
        // 2 triangles.....
        // therefore the number of triangles is the sum of the first n natural numbers, where n is all points - 2
        // 3 + 2 + 1
        int pointsLeft = (allPoints.Length - startIndex - 1);

        // If no 3 points in allPoints are in the same line. 
        int triangleCount = ((pointsLeft * (pointsLeft + 1)) / 2);

        UnsafeTriangle[] triangles = new UnsafeTriangle[triangleCount];
        int triangleTracker = -1;

        for (int i = startIndex; i < allPoints.Length; ++i)
        {
            Vector2 B = allPoints[i];

            // Ignore the parameter, we do not want to have a triangle where
            // two vertices are the same point.
            if (B == A)
            {
                continue;
            }

            for (int j = (i + 1); j < allPoints.Length; ++j)
            {
                ++triangleTracker;

                Vector2 C = allPoints[j];

                // For three point that are in line, we cannot construct a triangle,
                // leave the element null; we will filter it later.
                if (PointUtilities.AreThreePointsInLine(A, B, C))
                {
                    continue;
                }

                triangles[triangleTracker] = new UnsafeTriangle(A, B, C);

            }

        }

        // Return the whole array even with the nulls
        return triangles;
    }


}
