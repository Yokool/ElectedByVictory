using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    public class WorldCreator : MonoBehaviour
    {
        [SerializeField]
        private GameObject p_Province;

        [SerializeField]
        private int numberOfProvinces;

        [SerializeField]
        private float worldWidth;
        [SerializeField]
        private float worldHeight;
        
        [SerializeField]
        private int seedRadius;

        /// <summary>
        /// Move to some manager.
        /// </summary>
        private void OnEnable()
        {
            CreateWorld();
        }

        public void CreateWorld()
        {
            VoronoiSeed[] seeds = GenerateSeeds();
            GameObject[] provinces = CreateProvincesFromSeeds(seeds);

        }

        private GameObject[] CreateProvincesFromSeeds(VoronoiSeed[] seeds)
        {
            GameObject[] provinces = new GameObject[seeds.Length];
            for(int i = 0; i < seeds.Length; ++i)
            {
                VoronoiSeed seed = seeds[i];
                GameObject province = CreateProvinceFromSeed(seed, seeds);
                provinces[i] = province;
            }
            return provinces;
        }

        private GameObject CreateProvinceFromSeed(VoronoiSeed seed, VoronoiSeed[] allSeeds)
        {
            VoronoiSeedProvinceInit provinceInit = InstantiateProvinceFromSeed(seed, allSeeds);
            return provinceInit.gameObject;
        }

        private VoronoiSeedProvinceInit InstantiateProvinceFromSeed(VoronoiSeed seed, VoronoiSeed[] allSeeds)
        {
            GameObject r_Province = Instantiate(p_Province);

            VoronoiSeedProvinceInit provinceInit = r_Province.GetComponent<VoronoiSeedProvinceInit>();
            
            provinceInit.SetAssociatedSeed(seed);
            provinceInit.SetPositionFromSeed(this, seed);
            provinceInit.CreateMeshFromSeed(this, seed);

            VoronoiSeed[] otherSeeds = allSeeds.Where( (iteratedSeed) => { return iteratedSeed != seed; } ).ToArray();

            provinceInit.ClampVerticesToVoronoi(otherSeeds);

            return provinceInit;
        }

        /// <summary>
        /// MOVE TO <see cref="VoronoiSeed"/>
        /// </summary>
        /// <returns></returns>
        private VoronoiSeed[] GenerateSeeds()
        {
            VoronoiSeed[] seeds = new VoronoiSeed[numberOfProvinces];

            float xBound = worldWidth / 2f;
            float yBound = worldHeight / 2f;

            Vector3 worldCreatorPosition = gameObject.transform.position;
            xBound += worldCreatorPosition.x;
            yBound += worldCreatorPosition.y;

            for (int i = 0; i < seeds.Length; ++i)
            {
                float x = Random.Range(-xBound, xBound);
                float y = Random.Range(-yBound, yBound);

                seeds[i] = new VoronoiSeed(x, y, seedRadius);
            }
            return seeds;
        }

    }
}


namespace ElectedByVictory.WorldCreation
{
    public class VoronoiSeed
    {
        /// <summary>
        /// The x world position of the seed.
        /// </summary>
        private float x;
        /// <summary>
        /// The y world position of the seed.
        /// </summary>
        private float y;

        private Circle circleEquation;

        public VoronoiSeed(float x, float y, float circleRadius)
        {
            SetX(x);
            SetY(y);
            SetCircleEquation(circleRadius);
        }

        public void SetCircleEquation(float circleRadius)
        {
            this.circleEquation = new Circle(GetX(), GetY(), circleRadius);
        }

        public Circle GetCircle()
        {
            return this.circleEquation;
        }

        public void SetX(float x)
        {
            this.x = x;
            if(circleEquation != null)
            {
                circleEquation.SetX(x);
            }
            
        }

        public void SetY(float y)
        {
            this.y = y;
            if (circleEquation != null)
            {
                circleEquation.SetY(y);
            }
        }

        public float GetX()
        {
            return this.x;
        }

        public float GetY()
        {
            return this.y;
        }

    }

    public class Circle
    {
        private float x;
        private float y;
        private float radius;

        public Circle(float x, float y, float radius)
        {
            SetX(x);
            SetY(y);
            SetRadius(radius);
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        public void SetX(float x)
        {
            this.x = x;
        }

        public void SetY(float y)
        {
            this.y = y;
        }

        public float GetX()
        {
            return this.x;
        }

        public float GetY()
        {
            return this.y;
        }

        public float GetRadius()
        {
            return this.radius;
        }

        public Vector2 GetPointAtAngleDegrees(float angle)
        {
            angle = (float)(angle * (System.Math.PI / 180.0));
            return GetPointAtAngleRad(angle);
        }

        public Vector2 GetPointAtAngleRad(float angle)
        {
            float angleX = GetX() + GetRadius() * Mathf.Cos(angle);
            float angleY = GetY() + GetRadius() * Mathf.Sin(angle);

            return new Vector2(angleX, angleY);
        }



    }

}

