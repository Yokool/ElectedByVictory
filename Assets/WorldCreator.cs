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
       

        private List<VoronoiSeedManager> activeSeeds = new List<VoronoiSeedManager>();

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

        /*
        public VoronoiSeedData[] ExtractActiveSeedData()
        {
            List<VoronoiSeedManager> activeSeeds = GetActiveSeeds();
            
            VoronoiSeedData[] extractedData = new VoronoiSeedData[activeSeeds.Count];

            for(int i = 0; i < activeSeeds.Count; ++i)
            {
                VoronoiSeedManager activeSeed = activeSeeds[i];
                VoronoiSeedData seedData = activeSeed.GetVoronoiSeedData();

                extractedData[i] = seedData;
            }
            
            return extractedData;
        }

        public void UpdateAllSeedsAndTheirMesh()
        {
            InternalUpdateMethod( (seed, dataDeepCopy) => { seed.DELETE_UpdateSharedVoronoiWorldDataUpdate(dataDeepCopy); } );
        }

        public void UpdateAllSeedsOnlyData()
        {
            InternalUpdateMethod((seed, dataDeepCopy) => { seed.SetSharedVoronoiWorldData(dataDeepCopy); });
        }

        private void InternalUpdateMethod(FullSeedUpdateMethod internalUpdateMethod)
        {
            VoronoiSeedData[] extractedData = ExtractActiveSeedData();
            SharedVoronoiWorldData sharedData = new SharedVoronoiWorldData(extractedData, GetCornerData());

            List<VoronoiSeedManager> seeds = GetActiveSeeds();
            for (int i = 0; i < seeds.Count; ++i)
            {
                VoronoiSeedManager seed = seeds[i];
                SharedVoronoiWorldData dataDeepCopy = new SharedVoronoiWorldData(sharedData);
                internalUpdateMethod.Invoke(seed, dataDeepCopy);
            }
        }

        */

        public void CreateWorld()
        {
            SetupCorners();
            VoronoiSeedData[] fullVoronoiSeeds = GenerateRandomSeedData();
            AddSeedRangeToWorldNonUpdate(fullVoronoiSeeds);
            RebuildVoronoiWorld();
        }

        public void RebuildVoronoiWorld()
        {
            RebuildVoronoiObjects();
            RebuildVoronoiInsideData();
        }

        private void RebuildVoronoiInsideData()
        {
            List<VoronoiSeedManager> voronoiSeeds = GetActiveSeeds();
            for (int i = 0; i < voronoiSeeds.Count; ++i)
            {
                VoronoiSeedManager voronoiSeed = voronoiSeeds[i];
                voronoiSeed.RebuildSharedData(voronoiSeeds.ToArray(), GetCornerData());
            }
        }

        private void RebuildVoronoiObjects()
        {
            List<VoronoiSeedManager> voronoiSeeds = GetActiveSeeds();
            for (int i = 0; i < voronoiSeeds.Count; ++i)
            {
                VoronoiSeedManager voronoiSeed = voronoiSeeds[i];
                voronoiSeed.InstantiateFreshMeshObject();
            }
        }
        /*
        private void InstantiateMeshObjectsForAllSeeds()
        {
            List<VoronoiSeedManager> activeSeeds = GetActiveSeeds();
            for (int i = 0; i < activeSeeds.Count; ++i)
            {
                VoronoiSeedManager seed = activeSeeds[i];
                seed.InstantiateFreshMeshObject();
            }
        }
        */
        /*
        public VoronoiSeedManager[] GenerateFullRandomVoronoiSeeds()
        {
            VoronoiSeedData[] seeds = GenerateRandomSeedData();
            VoronoiSeedManager[] fullVoronoiSeeds = new VoronoiSeedManager[seeds.Length];

            for (int i = 0; i < seeds.Length; ++i)
            {
                VoronoiSeedData seed = seeds[i];
                fullVoronoiSeeds[i] = new VoronoiSeedManager(seed);
            }
            return fullVoronoiSeeds;
        }
        */
        /*
        public void AddVoronoiSeedToPlane(VoronoiSeedData seed)
        {
            activeSeeds.Add(seed);
            GameObject instantiatedSeed = GameObject.Instantiate(pSeedPrefab);
            VoronoiSeedProvinceInit initializiationScript = instantiatedSeed.GetComponent<VoronoiSeedProvinceInit>();
            initializiationScript.SetAssociatedSeed(seed);
        }
        */

        /*
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
        */

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

        private List<VoronoiSeedManager> GetActiveSeeds()
        {
            return this.activeSeeds;
        }

        private void AddSeedRangeToWorldNonUpdate(IEnumerable<VoronoiSeedData> voronoiSeedDataRange)
        {
            VoronoiSeedManager[] seedManagers = new VoronoiSeedManager[voronoiSeedDataRange.Count()];
            int i = 0;
            foreach(VoronoiSeedData voronoiSeedData in voronoiSeedDataRange)
            {
                seedManagers[i] = new VoronoiSeedManager(voronoiSeedData);
                ++i;
            }
            GetActiveSeeds().AddRange(seedManagers);
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


public delegate void FullSeedUpdateMethod(VoronoiSeedManager seedToUpdate, SharedVoronoiWorldData dataDeepCopy);