using UnityEngine;


namespace ElectedByVictory.WorldCreation
{
    public static class VertexMath
    {
        public static Vector2[] SortVerticesByAngleToPoint(Vector2[] vertices, Vector2 point)
        {
            Vector2[] verticesCopy = new Vector2[vertices.Length];
            vertices.CopyTo(vertices, 0);

            float[] angles = new float[verticesCopy.Length];

            // Calculate the angles for all points
            for(int i = 0; i < verticesCopy.Length; ++i)
            {
                Vector2 vertex = verticesCopy[i];
                angles[i] = PointMath.AngleBetweenPointsRad(vertex, point);
            }

            // Sorth them
            for(int i = 0; i < verticesCopy.Length; ++i)
            {
                for(int j = (i + 1); j < verticesCopy.Length; ++j)
                {
                    Vector2 currentVertex = verticesCopy[i];
                    float currentAngle = angles[i];

                    Vector2 otherVertex = verticesCopy[j];
                    float otherAngle = angles[j];

                    if(otherAngle < currentAngle)
                    {
                        verticesCopy[j] = currentVertex;
                        angles[j] = currentAngle;

                        verticesCopy[i] = otherVertex;
                        angles[i] = otherAngle;
                    }

                }
            }

            return verticesCopy;
        }
    }

}
