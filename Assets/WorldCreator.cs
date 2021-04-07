using ElectedByVictory.WorldCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    public class WorldCreator : MonoBehaviour
    {
        [SerializeField]
        private GameObject pSeedPrefab;

        [SerializeField]
        private int numberOfProvinces;

        [SerializeField]
        private float worldWidth;
        [SerializeField]
        private float worldHeight;
        
        [SerializeField]
        private int seedRadius;

        private CornerData cornerData;
       

        private List<FullVoronoiSeed> activeSeeds = new List<FullVoronoiSeed>();

        /// <summary>
        /// Move to some manager.
        /// </summary>
        private void OnEnable()
        {
            CreateWorld();
        }

        private void SetupCorners()
        {
            CornerData cornerData = new CornerData();
            cornerData.SetupCornersFromSquare(GetWorldCenter(), GetWorldWidth(), GetWorldHeight());
            SetCornerData(cornerData);
        }

        public VoronoiSeedData[] ExtractActiveSeedData()
        {
            List<FullVoronoiSeed> activeSeeds = GetActiveSeeds();
            
            VoronoiSeedData[] extractedData = new VoronoiSeedData[activeSeeds.Count];

            for(int i = 0; i < activeSeeds.Count; ++i)
            {
                FullVoronoiSeed activeSeed = activeSeeds[i];
                VoronoiSeedData seedData = activeSeed.GetVoronoiSeedData();

                extractedData[i] = seedData;
            }
            
            return extractedData;
        }

        public void UpdateAllSeedsAndTheirMesh()
        {
            InternalUpdateMethod( (seed, dataDeepCopy) => { seed.UpdateSharedVoronoiWorldDataUpdate(dataDeepCopy); } );
        }

        public void UpdateAllSeedsOnlyData()
        {
            InternalUpdateMethod((seed, dataDeepCopy) => { seed.UpdateSharedVoronoiWorldDataNonUpdate(dataDeepCopy); });
        }

        private void InternalUpdateMethod(FullSeedUpdateMethod internalUpdateMethod)
        {
            VoronoiSeedData[] extractedData = ExtractActiveSeedData();
            SharedVoronoiWorldData sharedData = new SharedVoronoiWorldData(extractedData, GetCornerData());

            List<FullVoronoiSeed> seeds = GetActiveSeeds();
            for (int i = 0; i < seeds.Count; ++i)
            {
                FullVoronoiSeed seed = seeds[i];
                SharedVoronoiWorldData dataDeepCopy = new SharedVoronoiWorldData(sharedData);
                internalUpdateMethod.Invoke(seed, dataDeepCopy);
            }
        }

        

        public void CreateWorld()
        {
            SetupCorners();
            GenerateFullRandomVoronoiSeeds();
            FullVoronoiSeed[] fullVoronoiSeeds = GenerateFullRandomVoronoiSeeds();
            AddSeedRangeUpdate(fullVoronoiSeeds);
            InstantiateMeshObjectsForAllSeeds();
            
            //GameObject[] provinces = CreateProvincesFromSeeds(seeds);
        }

        private void InstantiateMeshObjectsForAllSeeds()
        {
            List<FullVoronoiSeed> activeSeeds = GetActiveSeeds();
            for (int i = 0; i < activeSeeds.Count; ++i)
            {
                FullVoronoiSeed seed = activeSeeds[i];
                seed.InstantiateMeshObject();
            }
        }

        public FullVoronoiSeed[] GenerateFullRandomVoronoiSeeds()
        {
            VoronoiSeedData[] seeds = GenerateRandomSeedData();
            FullVoronoiSeed[] fullVoronoiSeeds = new FullVoronoiSeed[seeds.Length];

            for (int i = 0; i < seeds.Length; ++i)
            {
                VoronoiSeedData seed = seeds[i];
                fullVoronoiSeeds[i] = new FullVoronoiSeed(seed);
            }
            return fullVoronoiSeeds;
        }
        /*
        public void AddVoronoiSeedToPlane(VoronoiSeedData seed)
        {
            activeSeeds.Add(seed);
            GameObject instantiatedSeed = GameObject.Instantiate(pSeedPrefab);
            VoronoiSeedProvinceInit initializiationScript = instantiatedSeed.GetComponent<VoronoiSeedProvinceInit>();
            initializiationScript.SetAssociatedSeed(seed);
        }
        */

        private GameObject[] CreateProvincesFromSeeds(VoronoiSeedData[] seeds)
        {
            GameObject[] provinces = new GameObject[seeds.Length];
            for(int i = 0; i < seeds.Length; ++i)
            {
                VoronoiSeedData seed = seeds[i];
                GameObject province = CreateProvinceFromSeed(seed, seeds);
                provinces[i] = province;
            }
            return provinces;
        }

        private GameObject CreateProvinceFromSeed(VoronoiSeedData seed, VoronoiSeedData[] allSeeds)
        {
            VoronoiSeedProvinceInit provinceInit = InstantiateProvinceFromSeed(seed, allSeeds);
            return provinceInit.gameObject;
        }

        private VoronoiSeedProvinceInit InstantiateProvinceFromSeed(VoronoiSeedData seed, VoronoiSeedData[] allSeeds)
        {
            GameObject r_Province = Instantiate(pSeedPrefab);

            VoronoiSeedProvinceInit provinceInit = r_Province.GetComponent<VoronoiSeedProvinceInit>();
            
            provinceInit.SetAssociatedSeed(seed);
            provinceInit.SetPositionFromSeed(this, seed);
            //provinceInit.CreateMeshFromSeed(this, seed);

            //VoronoiSeed[] otherSeeds = allSeeds.Where( (iteratedSeed) => { return iteratedSeed != seed; } ).ToArray();

            //^^EVERYTHING ABOVE WORKS
            
            //provinceInit.ClampVerticesToVoronoi(otherSeeds);

            return provinceInit;
        }

        private VoronoiSeedData[] GenerateRandomSeedData()
        {
            VoronoiSeedData[] seeds = new VoronoiSeedData[numberOfProvinces];

            float halfWorldWidth = GetHalfWorldWidth();
            float halfWorldHeight = GetHalfWorldHeight();

            Vector3 worldCenter = GetWorldCenter();
            halfWorldWidth += worldCenter.x;
            halfWorldHeight += worldCenter.y;

            for (int i = 0; i < seeds.Length; ++i)
            {
                float x = Random.Range(-halfWorldWidth, halfWorldWidth);
                float y = Random.Range(-halfWorldHeight, halfWorldHeight);

                seeds[i] = new VoronoiSeedData(x, y, seedRadius);
            }
            return seeds;
        }

        

        public Vector2 GetWorldCenter()
        {
            return gameObject.transform.position;
        }

        public float GetWorldWidth()
        {
            return this.worldHeight;
        }

        public float GetWorldHeight()
        {
            return this.worldWidth;
        }

        public float GetHalfWorldWidth()
        {
            return this.worldWidth / 2f;
        }

        public float GetHalfWorldHeight()
        {
            return this.worldHeight / 2f;
        }    

        private List<FullVoronoiSeed> GetActiveSeeds()
        {
            return this.activeSeeds;
        }

        private void AddSeedRangeUpdate(IEnumerable<FullVoronoiSeed> fullVoronoiSeeds)
        {
            AddSeedRangeNonUpdate(fullVoronoiSeeds);
            UpdateAllSeedsAndTheirMesh();
        }

        private void AddSeedRangeNonUpdate(IEnumerable<FullVoronoiSeed> fullVoronoiSeeds)
        {
            GetActiveSeeds().AddRange(fullVoronoiSeeds);
        }

        public CornerData GetCornerData()
        {
            return this.cornerData;
        }

        public void SetCornerData(CornerData cornerData)
        {
            this.cornerData = cornerData;
        }

    }
}


public delegate void FullSeedUpdateMethod(FullVoronoiSeed seedToUpdate, SharedVoronoiWorldData dataDeepCopy);