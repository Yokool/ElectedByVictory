using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    /// <summary>
    /// A class used to set up the province object from a voronoi seed instance
    /// </summary>
    public class VoronoiSeedProvinceInit : MonoBehaviour
    {
        [SerializeField]
        private int seedVertexCount;

        private PolygonCollider2D polygonCollider2D;
        private MeshFilter provinceMeshFilter;

        private VoronoiSeed associatedSeed;


        private void OnEnable()
        {
            polygonCollider2D = GetComponent<PolygonCollider2D>();
            provinceMeshFilter = GetComponent<MeshFilter>();
        }

        private void Awake()
        {
            // To check if the seed is 0 or negative when put through the editor
            SetSeedVertexCount(seedVertexCount);
        }

        public void SetSeedVertexCount(int seedVertexCount)
        {
            // We can't have 0 vertices, lets have at least one vertex which is the center of the object
            if (seedVertexCount <= 1)
            {
                Debug.LogWarning($"{nameof(seedVertexCount)} can not be smaller or equal to than 1.");
                seedVertexCount = 2;
            }

            int remainder = (seedVertexCount + 1) % 3;
            
            if (remainder != 0)
            {
                Debug.LogWarning($"{nameof(seedVertexCount)}: ({seedVertexCount}) + 1 must be divisible by 3.");
                seedVertexCount += remainder;
            }


            this.seedVertexCount = seedVertexCount;
        }

        public void SetAssociatedSeed(VoronoiSeed associatedSeed)
        {
            this.associatedSeed = associatedSeed;
        }

        public void SetPositionFromSeed(WorldCreator worldCreator, VoronoiSeed seed)
        {
            Vector3 provincePosition = Vector3.zero;
            provincePosition.x = seed.GetX();
            provincePosition.y = seed.GetY();

            // Set depth to be the same as the WorldCreator
            provincePosition.z = worldCreator.gameObject.transform.position.z;

            gameObject.transform.position = provincePosition;
        }

        private Vector3[] CreateCircleWorldVertices(VoronoiSeed seed)
        {
            Circle circleSeed = seed.GetCircle();

            Vector3[] circleVertices = new Vector3[seedVertexCount];

            float step = 360f / seedVertexCount;

            // Relative to the province, the z vertex should be 0
            // The final world pos of the vertice will be the same as the z position of the WorldCreator
            float z = gameObject.transform.position.z;

            for (int i = 0; i < circleVertices.Length; ++i)
            {
                float currentAngle = i * step;
                Vector2 point = circleSeed.GetPointAtAngleDegrees(currentAngle);

                Vector2 worldSpacePoint = new Vector3(point.x, point.y, z);

                circleVertices[i] = worldSpacePoint;
            }

            return circleVertices;

        }

        private Vector3[] CreateMeshVertices(Vector3[] circleVertices)
        {

            Vector3[] meshVertices = new Vector3[circleVertices.Length];

            // Center relative to WorldCreator

            Vector3 gameObjectPosition = gameObject.transform.position;

            //meshVertices[0] = new Vector3(gameObjectPosition.x, gameObjectPosition.y, gameObjectPosition.z);

            for (int i = 0; i < meshVertices.Length; ++i)
            {
                meshVertices[i] = circleVertices[i];
            }

            return meshVertices;
        }

        private void SetupColliderComponent(Vector3[] circleVertices)
        {
            Vector2[] _2DVertices = new Vector2[circleVertices.Length];

            for (int i = 0; i < _2DVertices.Length; ++i)
            {
                Vector3 _3DVertex = circleVertices[i];
                Vector2 _2DVertex = new Vector2(_3DVertex.x, _3DVertex.y);
                _2DVertices[i] = _2DVertex;
            }

            polygonCollider2D.points = _2DVertices;
        }

        private void SetMeshFilter(Vector3[] vertices, Vector2[] uvs, int[] triangles)
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            provinceMeshFilter.mesh = mesh;
        }

        private Vector2[] CreateMeshUVs(Vector3[] localSpaceMeshVertices)
        {
            Vector2[] uvs = new Vector2[localSpaceMeshVertices.Length];

            float maxX = -float.MaxValue;
            float maxY = -float.MaxValue;

            for (int i = 0; i < localSpaceMeshVertices.Length; ++i)
            {
                Vector3 vertex = localSpaceMeshVertices[i];
                float vertex_x = vertex.x;
                float vertex_y = vertex.y;

                if (vertex_x > maxX)
                {
                    maxX = vertex_x;
                }

                if (vertex_y > maxY)
                {
                    maxY = vertex_y;
                }
            }

            for (int i = 0; i < uvs.Length; ++i)
            {
                Vector3 uvVertex = localSpaceMeshVertices[i];
                float relativeX = uvVertex.x / maxX;
                float relativeY = uvVertex.y / maxY;

                Vector2 uv = new Vector2(relativeX, relativeY);
                uvs[i] = uv;
            }

            return uvs;
        }

        private int[] CreateMeshTriangles(Vector3[] localSpaceMeshVertices)
        {
            int triangleNumber = localSpaceMeshVertices.Length - 1;
            int[] triangles = new int[triangleNumber * 3];

            //Unity does triangles order matter

            // THE MESHES ARE VISIBLE FROM THE OTHER SIDE!

            /*
             * 
             *   The order of the vertex array is irrelevant; the order of the triangle array
             *   determines how triangles are rendered. For example, if [1, 2, 3] refers to vertices in a clockwise order,
             *   then [1, 3, 2] is counter-clockwise and the surface normal is reversed. 
             * 
             *
             */
            int header = 1;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                triangles[i] = 0;
                triangles[i + 1] = header + 1;
                triangles[i + 2] = header;

                ++header;
            }

            triangles[triangles.Length - 3] = 0;
            triangles[triangles.Length - 2] = 1;
            triangles[triangles.Length - 1] = header - 1;

            return triangles;

        }

        public void CreateMeshFromSeed(WorldCreator worldCreator, VoronoiSeed seed)
        {
            Vector3[] circleWorldVertices = CreateCircleWorldVertices(seed);
            Vector3[] localSpaceMeshVertices = transform.InverseTransformPoints(circleWorldVertices);
            Vector2[] uvs = CreateMeshUVs(localSpaceMeshVertices);
            int[] triangles = CreateMeshTriangles(localSpaceMeshVertices);

            

            SetMeshFilter(localSpaceMeshVertices, uvs, triangles);

            SetupColliderComponent(localSpaceMeshVertices);

        }

        public void ClampVerticesToVoronoi(VoronoiSeed[] otherSeeds)
        {
            Line[] clampingLines = GetClampingLines(otherSeeds);

            Mesh provinceMesh = provinceMeshFilter.mesh;
            Vector3[] worldSpaceVertices = transform.TransformPoints(provinceMesh.vertices);

            Vector2 origin = gameObject.transform.position;

            // TODO: REPLACE WITH A .SELECT() METHOD
            for(int i = 0; i < worldSpaceVertices.Length; ++i)
            {
                Vector3 worldSpaceVertex = worldSpaceVertices[i];

                for(int j = 0; j < clampingLines.Length; ++j)
                {
                    Line clampingLine = clampingLines[j];

                    // TODO: FIX THE FUCKERY IN HERE
                    // TODO: YOU NEED TO TRANSFORMPOINT AND TRANSFORM INVERSE THIS BITCH
                    // TODO: BE MINDFUL IN WHAT COORDINATE SYSTEM ARE YOU WORKING
                    worldSpaceVertex = PointUtilities.ClampPointToLine(origin, worldSpaceVertex, clampingLine);
                }

                worldSpaceVertices[i] = worldSpaceVertex;
            }

            provinceMesh.vertices = transform.InverseTransformPoints(worldSpaceVertices);

        }

        public Line[] GetLinesToVertices()
        {
            Mesh provinceMesh = provinceMeshFilter.mesh;
            Vector3[] vertices = provinceMesh.vertices;

            Line[] lineFromOriginToVertices = new Line[vertices.Length];

            Vector2 thisPos = gameObject.transform.position;
            float t_x = thisPos.x;
            float t_y = thisPos.y;

            for(int i = 0; i < lineFromOriginToVertices.Length; ++i)
            {
                Vector2 vertex = vertices[i];
                Line lineFromOriginToVertice = new Line(t_x, t_y, vertex.x, vertex.y);
                lineFromOriginToVertices[i] = lineFromOriginToVertice;
            }

            return lineFromOriginToVertices;
        }

        List<Vector3> clampingLinesGizmos = new List<Vector3>();
        /*
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 2f);

            MeshFilter mf = GetComponent<MeshFilter>();
            Vector3[] vert = mf.mesh.vertices;
            for (int i = 0; i < vert.Length; ++i)
            {
                Gizmos.DrawSphere(transform.TransformPoint(vert[i]), 1f);
            }

        }
        */
        /// <summary>
        /// I'm 90% sure this works.
        /// </summary>
        /// <param name="otherSeeds"></param>
        /// <returns></returns>
        public Line[] GetClampingLines(VoronoiSeed[] otherSeeds)
        {
            Line[] perpendicularLines = new Line[otherSeeds.Length];

            for (int i = 0; i < otherSeeds.Length; ++i)
            {
                VoronoiSeed seed = otherSeeds[i];

                Vector2 worldOriginPos = gameObject.transform.position;

                float originX = worldOriginPos.x;
                float originY = worldOriginPos.y;

                float seedCenterX = seed.GetX();
                float seedCenterY = seed.GetY();

                // A line segment starting at the center of this obj and ending at the center of the other seed.
                LineSegment thisCenterToSeedCenterLine = new LineSegment(originX, originY, seedCenterX, seedCenterY);

                clampingLinesGizmos.Add(new Vector3(originX, originY));
                clampingLinesGizmos.Add(new Vector3(seedCenterX, seedCenterY));

                //Debug.Log(originX);
                //Debug.Log(originY);

                /*
                Debug.Log(thisCenterToSeedCenterLine);
                Debug.Log($"t_x: {originX} t_y: {originY} s_x: {seedCenterX} s_y: {seedCenterY}");
                Debug.Log(thisCenterToSeedCenterLine.GetPointAt(0.5f));
                */

                // Get a line perpendicular to this line, going through a point half through this line segment.
                perpendicularLines[i] = thisCenterToSeedCenterLine.GetLinePerpendicularAtWay(0.5f);

            }
            return perpendicularLines;
        }

        public VoronoiSeed GetAssociatedSeed()
        {
            return this.associatedSeed;
        }    

    }



}
