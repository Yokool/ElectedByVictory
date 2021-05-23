using System;
using UnityEngine;


namespace ElectedByVictory.WorldCreation
{

    public class VoronoiSeedManager
    {

        private VoronoiSeedData voronoiSeedData;

        private GameObject rMeshObject;

        //private SharedVoronoiWorldData sharedData;

        private VoronoiBuildData voronoiBuildData;


        public VoronoiSeedManager(VoronoiSeedData voronoiSeedData)
        {
            SetVoronoiSeedData(voronoiSeedData);
        }

        public void RebuildVertices()
        {
            GameObject rMeshObject = GetMeshObject();
            
            Vector2[] _2Dvertices = GetVoronoiBuildData().GetVoronoiCellVertices();

            if (_2Dvertices.Length < 3)
            {
                
                Debug.Log("============");
                ILineRaySegmentUnion[] edges = voronoiBuildData.GetVoronoiCellEdges();
                Debug.Log(edges.Length);
                for (int i = 0; i < edges.Length; ++i)
                {
                    Debug.Log(edges[i]);
                }
                Debug.Log("============");
                
                return;
            }

            int[] triangles = ConvexMeshCalculator.GetTrianglesForConvexVertices(_2Dvertices);
            Vector2[] UVs = ConvexMeshCalculator.GetUVs(_2Dvertices);

            Vector3[] _3DVerticesAtObjectDepth = PointUtilities.Cast2DVerticesTo3DAndSetZ(_2Dvertices, rMeshObject.transform.position.z);

            _3DVerticesAtObjectDepth = rMeshObject.transform.InverseTransformPoints(_3DVerticesAtObjectDepth);

            Mesh m = new Mesh();
            m.vertices = _3DVerticesAtObjectDepth;
            m.triangles = triangles;
            m.uv = UVs;

            rMeshObject.GetComponent<MeshFilter>().mesh = m;
        }

        private void SetVoronoiBuildData(VoronoiBuildData voronoiBuildData)
        {
            this.voronoiBuildData = voronoiBuildData;
        }

        private VoronoiBuildData GetVoronoiBuildData()
        {
            return this.voronoiBuildData;
        }

        private void SetVoronoiSeedData(VoronoiSeedData voronoiSeedData)
        {
            this.voronoiSeedData = voronoiSeedData;
        }

        /*
        private void SetVoronoiSeedDataUpdate(VoronoiSeedData voronoiSeedData)
        {
            SetVoronoiSeedDataNonUpdate(voronoiSeedData);
            TryUpdateMesh();
        }
        */
        /*
        private void TryUpdateMesh()
        {
            if(IsReadyForMeshConstruction())
            {
                UpdateMeshObjectUnsafe();
            }
        }
        */
        /*
        public void DELETE_UpdateSharedVoronoiWorldDataUpdate(SharedVoronoiWorldData sharedData)
        {
            SetSharedVoronoiWorldData(sharedData);
            TryUpdateMesh();
        }

        
        public void SetSharedVoronoiWorldData(SharedVoronoiWorldData sharedData)
        {
            this.sharedData = sharedData;
        }

        */

        public VoronoiSeedData GetVoronoiSeedData()
        {
            return this.voronoiSeedData;
        }

        /*
        private SharedVoronoiWorldData GetSharedData()
        {
            return this.sharedData;
        }
        */
        

        private void SetMeshObject(GameObject rMeshObject)
        {
            this.rMeshObject = rMeshObject;
        }

        private GameObject GetMeshObject()
        {
            return this.rMeshObject;
        }

        public void InstantiateFreshMeshObject()
        {
            // Clear the last mesh object
            if(HasMeshObject())
            {
                GameObject.Destroy(GetMeshObject());
                SetMeshObject(null);
            }

            GameObject rMeshObject = GameObject.Instantiate(GameResources.GET_INSTANCE().GetVoronoiMeshObject());
            rMeshObject.transform.position = GetVoronoiSeedData().GetPosition();
            SetMeshObject(rMeshObject);
        }

        public bool HasMeshObject()
        {
            return (GetMeshObject() != null);
        }

        /*
        /// <summary>
        /// Return true, if this object is fully setup and has all the data necessary to receive mesh
        /// calculation requests.
        /// </summary>
        /// <returns></returns>
        public bool IsReadyForCalculation()
        {
            return (GetSharedData() != null);
        }
        */
        /*
        public bool IsReadyForMeshConstruction()
        {
            return (IsReadyForCalculation() && HasMeshObject());
        }
        */

        public void RebuildSharedData(VoronoiSeedManager[] voronoiSeeds, CornerData cornerData)
        {
            SetVoronoiBuildData(new VoronoiBuildData(this, voronoiSeeds, cornerData));
        }

        /*
        /// <summary>
        /// 
        /// Documentation (version 1) - Last Update: 26.3.2021 - 11:52
        /// 
        /// Updates the vertices of the associated mesh objects.
        /// Marked as unsafe, since you can call this method on an <see cref="VoronoiSeedManager"/> object without
        /// a reference to an associated mesh object, which will result in a null pointer exception. The
        /// method is also unsafe due to the <see cref="GetSharedData"/> field, which might
        /// not have data.
        /// 
        /// If you want to be sure that you can call this method check true for <see cref="IsReadyForMeshConstruction"/>
        /// 
        /// You can check whether this object has an associated mesh <see cref="GameObject"/> through
        /// the method <see cref="HasMeshObject"/>.
        /// 
        /// </summary>
        public void UpdateMeshObjectUnsafe()
        {
            GameObject meshObject = GetMeshObject();
            meshObject.transform.position = GetVoronoiSeedData().GetPosition();

            VoronoiMeshCalculator thisCalculator = new VoronoiMeshCalculator(GetVoronoiSeedData(), GetSharedData());
            
            Vector2[] vertices2D = thisCalculator.GetVertices();
            int[] triangles = ConvexMeshCalculator.GetTrianglesForConvexVertices(vertices2D);
            Vector2[] uvs = ConvexMeshCalculator.GetUVs(vertices2D);

            Vector3[] vertices3D = Transform2DVerticesTo3D(vertices2D);
            
            vertices3D = meshObject.transform.InverseTransformPoints(vertices3D);
            
            Mesh cellMesh = new Mesh();
            cellMesh.vertices = vertices3D;
            cellMesh.triangles = triangles;
            cellMesh.uv = uvs;

            MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
            meshFilter.mesh = cellMesh;

        }
        */

        private Vector3[] Transform2DVerticesTo3D(Vector2[] vertices)
        {
            GameObject meshObject = GetMeshObject();
            float meshObjectZ = meshObject.transform.position.z;

            Vector3[] vertices3D = new Vector3[vertices.Length];
            
            for(int i = 0; i < vertices.Length; ++i)
            {
                Vector2 vertex2D = vertices[i];
                vertices3D[i] = new Vector3(vertex2D.x, vertex2D.y, meshObjectZ);
            }

            return vertices3D;
        }

    }

}
