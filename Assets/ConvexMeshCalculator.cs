using UnityEngine;


namespace ElectedByVictory.WorldCreation
{
    public static class ConvexMeshCalculator
    {

        public static Vector2[] GetUVs(Vector2[] vertices)
        {
            if(vertices.Length < 3)
            {
                Debug.LogError("Shouldn't calculate vertices for vertex list that has less than 3 members.");
                return new Vector2[0];
            }

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            for(int i = 0; i < vertices.Length; ++i)
            {
                Vector2 vertex = vertices[i];

                if(vertex.x > maxX)
                {
                    maxX = vertex.x;
                }

                if(vertex.y > maxY)
                {
                    maxY = vertex.y;
                }

            }

            Vector2[] uvs = new Vector2[vertices.Length];

            for(int i = 0; i < vertices.Length; ++i)
            {
                Vector2 vertex = vertices[i];

                float x = (vertex.x / maxX);
                float y = (vertex.y / maxY);

                uvs[i] = new Vector2(x, y);
            }

            return uvs;

        }

        public static int[] GetTrianglesForConvexVertices(Vector2[] vertices)
        {

            if(vertices.Length < 3)
            {
                Debug.LogError("Can't construct triangles for a list of vertices whose length is smaller than 3.");
                return new int[0];
            }

            int triangleCount = (vertices.Length - 2);
            int triangleEdgeCount = (triangleCount * 3);

            int[] triangleEdges = new int[triangleEdgeCount];

            int triangleIndex = 0;

            for(int i = 1; i < (vertices.Length - 1); i++)
            {
                triangleEdges[triangleIndex] = 0;
                ++triangleIndex;

                triangleEdges[triangleIndex] = i;
                ++triangleIndex;

                triangleEdges[triangleIndex] = (i + 1);
                ++triangleIndex;

            }

            return triangleEdges;
        }

    }

}
