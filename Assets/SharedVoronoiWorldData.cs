using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{


    public class SharedVoronoiWorldData
    {
        private VoronoiSeedData[] allSeeds;

        private CornerData cornerData;

        /// <summary>
        /// 
        /// Documentation (version 1) - Last Update: 28.3.2021 13:17
        /// 
        /// In order to get the midpoint line going from a seed to another seed, we use <see cref="GetMidpointLineUtilFromTo(VoronoiSeedData, VoronoiSeedData)"/>.
        /// You will see that the first <see cref="VoronoiSeedData"/> key accesses an underlying dictionary, which itself uses
        /// a <see cref="VoronoiSeedData"/> key in order to access the <see cref="Line"/> object. We are getting a midpoint line, which is a line that is perpendicular
        /// and at the halfway point of a line segment whose endpoints are the first and second key.
        /// 
        /// If we access the underlying dictionary using the first <see cref="VoronoiSeedData"/> key. It does not matter what key
        /// we use. We will always get a midpoint <see cref="Line"/> for a line segment, whose one endpoint is that of the seed by
        /// which we have accessed the underlying dictionary.
        /// 
        /// The second endpoint is formed by the second key.
        /// 
        /// If you were to enumerate all of the keys in the underlying dictionary, which you've accessed with seed1. You would
        /// get all the lines going from seed1 to all of the other seeds.
        /// 
        /// The <see cref="midpointLineFromToDictionary"/> field is constructed throgh <see cref="CalculateMidpointLinesForAllPoints"/>.
        /// 
        /// </summary>
        /// 
        private Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>> midpointLineFromToDictionary;

        public SharedVoronoiWorldData(VoronoiSeedData[] allSeeds, CornerData cornerData)
        {
            CopyAllSeedsFromArrayToNewArray(allSeeds);
            SetCornerData(cornerData);
            CalculateMidpointLinesForAllPoints();
        }

        public SharedVoronoiWorldData(VoronoiSeedData[] allSeeds, CornerData cornerData,
            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>> cachedMidpointLineUtils)
        {
            CopyAllSeedsFromArrayToNewArray(allSeeds);
            SetCornerData(cornerData);
            SetMidpointLineFromToDictionary(cachedMidpointLineUtils);
        }

        /// <summary>
        /// This is different to using the assignment operator, as it does a deep copy of the registeredSeeds array.
        /// </summary>
        /// <param name="data"></param>
        public SharedVoronoiWorldData(SharedVoronoiWorldData data) : this(data.GetAllSeeds(), data.GetCornerData(), data.GetMidpointLineUtilDictionary())
        {

        }

        public MidpointLineUtil GetMidpointLineUtilFromTo(VoronoiSeedData from, VoronoiSeedData to)
        {
            Dictionary<VoronoiSeedData, MidpointLineUtil> midpointLineDictionaryFrom = GetMidpointLineUtilDictionaryFrom(from);

            if(!midpointLineDictionaryFrom.ContainsKey(to))
            {
                Debug.LogError("Dictionary for midpoint lines from seed " + from + "\n doesn't have a midpoint line for seed " + to);
                return null;
            }

            return midpointLineDictionaryFrom[to];
        }

        public bool TryGetMidpointLineUtilFromToBothway(VoronoiSeedData from, VoronoiSeedData to, out MidpointLineUtil midpointLineUtil)
        {
            midpointLineUtil = null;

            if(_TryGetMidpointLineUtilFromToOneway(from, to, out MidpointLineUtil leftToRightParameter))
            {
                midpointLineUtil = leftToRightParameter;
                return true;
            }

            if(_TryGetMidpointLineUtilFromToOneway(to, from, out MidpointLineUtil rightToLeftParameter))
            {
                midpointLineUtil = rightToLeftParameter;
                return true;
            }

            return false;
        }

        private bool _TryGetMidpointLineUtilFromToOneway(VoronoiSeedData from, VoronoiSeedData to, out MidpointLineUtil midpointLineUtil)
        {
            midpointLineUtil = null;

            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>> midpointLineUtilDictionary = GetMidpointLineUtilDictionary();
            
            bool containsFrom = midpointLineUtilDictionary.ContainsKey(from);
            
            if(!containsFrom)
            {
                return false;
            }

            Dictionary<VoronoiSeedData, MidpointLineUtil> midpointDictionaryFrom = GetMidpointLineUtilDictionaryFrom(from);
            bool containsTo = midpointDictionaryFrom.ContainsKey(to);

            if(!containsTo)
            {
                return false;
            }

            midpointLineUtil = GetMidpointLineUtilFromTo(from, to);
            return true;
        }
        /*
        public Line[] GetAllMidpointLinesNotToOrFromSeed(VoronoiSeedData seedToOmit)
        {
            List<Line> linesNotFromSeed = new List<Line>();

            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>> midpointLineUtilDictionary = GetMidpointLineUtilDictionary();

            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>>.KeyCollection upperLayerKeys = midpointLineUtilDictionary.Keys;

            foreach (VoronoiSeedData endpoint1 in upperLayerKeys)
            {
                if(endpoint1.Equals(seedToOmit))
                {
                    continue;
                }

                Dictionary<VoronoiSeedData, Line> midpointDictionaryLowerLayer = GetMidpointLineUtilDictionaryFrom(endpoint1);

                Dictionary<VoronoiSeedData, Line>.KeyCollection lowerLayerKeys = midpointDictionaryLowerLayer.Keys;

                foreach(VoronoiSeedData endpoint2 in lowerLayerKeys)
                {
                    if (endpoint2.Equals(seedToOmit))
                    {
                        continue;
                    }

                    Line midpointLine = GetMidpointLineUtilFromTo(endpoint1, endpoint2);

                    // So we don't get duplicate entries for the same line, since we're getting
                    // the lines from both sides of the dictionary (key combinatorics).
                    if(linesNotFromSeed.Contains(midpointLine))
                    {
                        continue;
                    }

                    linesNotFromSeed.Add(midpointLine);

                }

            }

            return linesNotFromSeed.ToArray();
        }
        */
        public VoronoiSeedData GetClosestSeedFromAllSeedsTo(VoronoiSeedData seed)
        {
            float seedX = seed.GetX();
            float seedY = seed.GetY();

            float minDistance = float.MaxValue;

            int savedIndex = -1;

            SeedConsumerWIndex findClosestSeed = (otherSeed, otherIndex) =>
            {
                float otherX = otherSeed.GetX();
                float otherY = otherSeed.GetY();

                float distance = MathEBV.PointDistance(seedX, seedY, otherX, otherY);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    savedIndex = otherIndex;
                }
            };
            EnumerateAllButSeedWIndex(seed, findClosestSeed);

            return GetAllSeeds()[savedIndex];
        }

        private Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>> GetMidpointLineUtilDictionary()
        {
            return this.midpointLineFromToDictionary;
        }

        private void SetMidpointLineFromToDictionary(Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>> midpointLineFromToDictionary)
        {
            this.midpointLineFromToDictionary = midpointLineFromToDictionary;
        }

        private void InitMidpointLineFromToDictionary()
        {
            SetMidpointLineFromToDictionary(new Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>>());
            VoronoiSeedData[] allSeeds = GetAllSeeds();

            for (int i = 0; i < allSeeds.Length; ++i)
            {
                VoronoiSeedData seed = allSeeds[i];
                InitMidpointLinesFromToForSingleSeed(seed);
            }
        }

        private void InitMidpointLinesFromToForSingleSeed(VoronoiSeedData seed)
        {
            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, MidpointLineUtil>> midpointLinesFromTo = GetMidpointLineUtilDictionary();
            midpointLinesFromTo.Add(seed, new Dictionary<VoronoiSeedData, MidpointLineUtil>());
        }

        public Dictionary<VoronoiSeedData, MidpointLineUtil> GetMidpointLineUtilDictionaryFrom(VoronoiSeedData seed)
        {
            return GetMidpointLineUtilDictionary()[seed];
        }

        private void EstablishMidpointLineUtilFromTo(VoronoiSeedData from, VoronoiSeedData to)
        {
            Vector2 fromPosition = from.GetPosition();
            Vector2 toPosition = to.GetPosition();
            MidpointLineUtil newLink = new MidpointLineUtil(fromPosition, toPosition);
            EstablishMidpointLineUtilFromToExistingReference(from, to, newLink);
        }

        private void EstablishMidpointLineUtilFromToExistingReference(VoronoiSeedData from, VoronoiSeedData to, MidpointLineUtil existingLink)
        {
            GetMidpointLineUtilDictionaryFrom(from).Add(to, existingLink);
        }

        private void CalculateMidpointLinesForAllPoints()
        {
            InitMidpointLineFromToDictionary();

            VoronoiSeedData[] allSeeds = GetAllSeeds();

            for(int i = 0; i < allSeeds.Length; ++i)
            {
                VoronoiSeedData seedToGenerateFor = allSeeds[i];

                for(int j = 0; j < allSeeds.Length; ++j)
                {
                    // Do not construct a line from itself to itself
                    if(j == i)
                    {
                        continue;
                    }

                    VoronoiSeedData otherSeed = allSeeds[j];

                    
                    // Try to see if the other seed is already linked to the seed we are generating for
                    if(_TryGetMidpointLineUtilFromToOneway(otherSeed, seedToGenerateFor, out MidpointLineUtil existingLineUtil))
                    {
                        EstablishMidpointLineUtilFromToExistingReference(seedToGenerateFor, otherSeed, existingLineUtil);
                        continue;
                    }

                    EstablishMidpointLineUtilFromTo(seedToGenerateFor, otherSeed);

                    /*
                    float otherX = otherSeed.GetX();
                    float otherY = otherSeed.GetY();

                    // A line segment starting at the center of this obj and ending at the center of the other seed.
                    LineSegment thisCenterToSeedCenterLine = new LineSegment(seedToGenerateFor.GetX(), seedToGenerateFor.GetY(), otherX, otherY);

                    Vector2 midpoint = thisCenterToSeedCenterLine.GetPointAt(0.5f);

                    // Get a line perpendicular to this line, going through a point half through this line segment.
                    Line midpointLine = thisCenterToSeedCenterLine.GetLinePerpendicularAtWay(0.5f);

                    AddMidpointLineFromTo(seedToGenerateFor, otherSeed, midpointLine);
                    AddMidpointLineToMidpointEntry(midpointLine, midpoint);
                    */
                }
            }
        }

        public MidpointLineUtil[] GetAllMidpointLineUtilsFrom(VoronoiSeedData seed)
        {
            Dictionary<VoronoiSeedData, MidpointLineUtil> allLinesFromSeedToOtherDictionary = GetMidpointLineUtilDictionaryFrom(seed);
            return allLinesFromSeedToOtherDictionary.Values.ToArray();
        }

        private void CopyAllSeedsFromArrayToNewArray(VoronoiSeedData[] allSeeds)
        {
            this.allSeeds = new VoronoiSeedData[allSeeds.Length];
            for (int i = 0; i < allSeeds.Length; ++i)
            {
                this.allSeeds[i] = allSeeds[i];
            }
        }

        private void SetCornerData(CornerData cornerData)
        {
            this.cornerData = cornerData;
        }

        public CornerData GetCornerData()
        {
            return this.cornerData;
        }

        public VoronoiSeedData[] GetAllSeeds()
        {
            return this.allSeeds;
        }

        public void EnumerateAllButSeed(VoronoiSeedData seedToOmit, SeedConsumer consumer)
        {
            SeedConsumerWIndex indexToNormalConsumer = (otherSeed, otherIndex) =>
            {
                consumer.Invoke(otherSeed);
            };

            EnumerateAllButSeedWIndex(seedToOmit, indexToNormalConsumer);
        }

        public void EnumerateAllButSeedWIndex(VoronoiSeedData seedToOmit, SeedConsumerWIndex seedConsumerWIndex)
        {
            for (int i = 0; i < allSeeds.Length; ++i)
            {
                VoronoiSeedData seed = allSeeds[i];
                if (seed.Equals(seedToOmit))
                {
                    continue;
                }
                seedConsumerWIndex.Invoke(seed, i);
            }
        }

    }

    public delegate void SeedConsumer(VoronoiSeedData seed);
    public delegate void SeedConsumerWIndex(VoronoiSeedData seed, int seedIndex);
}

