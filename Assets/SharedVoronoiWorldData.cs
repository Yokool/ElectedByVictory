using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    public class MidpointLine
    {
        private Line line;
        private Vector2 midpoint;

        public MidpointLine(Line line, Vector2 midpoint)
        {
            SetLine(line);
            SetMidpoint(midpoint);
        }

        private void SetLine(Line line)
        {
            this.line = line;
        }

        private void SetMidpoint(Vector2 midpoint)
        {
            this.midpoint = midpoint;
        }

        public Line GetLine()
        {
            return this.line;
        }

        public Vector2 GetMidpoint()
        {
            return this.midpoint;
        }

    }

    public class SharedVoronoiWorldData
    {
        private VoronoiSeedData[] allSeeds;

        private CornerData cornerData;

        // TODO: THERE ARE TWO ENTRIES IN THE LINE DICTIONARY
        // TODO: WE NEED TO MAKE IT SO BOTH WAYS YOU GET THE SAME LINE
        // TODO: SINCE LINE IS AN OBJECT JUST COPY THE REFERENCE OR SOMETHING

        // FROM(LINE1).TO(LINE2)
        // SHOULD RETURN THE SAME OBJECT AS
        // FROM(LINE2).TO(LINE1)

        /// <summary>
        /// 
        /// Documentation (version 1) - Last Update: 28.3.2021 13:17
        /// 
        /// In order to get the midpoint line going from a seed to another seed, we use <see cref="GetMidpointLineFromTo(VoronoiSeedData, VoronoiSeedData)"/>.
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
        private Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>> midpointLineFromToDictionary;

        private Dictionary<Line, Vector2> midpointLineToMidpointDictionary;

        public SharedVoronoiWorldData(VoronoiSeedData[] allSeeds, CornerData cornerData)
        {
            CopyAllSeedsFromArrayToNewArray(allSeeds);
            SetCornerData(cornerData);
            CalculateMidpointLinesForAllPoints();
        }

        public SharedVoronoiWorldData(VoronoiSeedData[] allSeeds, CornerData cornerData, Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>> cachedMidpointLines, Dictionary<Line, Vector2> cachedMidpointLineToMidpointDictionary)
        {
            CopyAllSeedsFromArrayToNewArray(allSeeds);
            SetCornerData(cornerData);
            SetMidpointLineFromToDictionary(cachedMidpointLines);
            SetMidpointLineToMidpointDictionary(cachedMidpointLineToMidpointDictionary);
        }

        /// <summary>
        /// This is different to using the assignment operator, as it does a deep copy of the registeredSeeds array.
        /// </summary>
        /// <param name="data"></param>
        public SharedVoronoiWorldData(SharedVoronoiWorldData data) : this(data.GetAllSeeds(), data.GetCornerData(), data.GetMidpointLineFromToDictionary(), data.GetMidpointLineToMidpointDictionary())
        {

        }

        public Vector2 GetMidpointFromMidpointLine(Line midpointLine)
        {
            return midpointLineToMidpointDictionary[midpointLine];
        }

        private Dictionary<Line, Vector2> GetMidpointLineToMidpointDictionary()
        {
            return this.midpointLineToMidpointDictionary;
        }

        private void SetMidpointLineToMidpointDictionary(Dictionary<Line, Vector2> midpointLineToMidpointDictionary)
        {
            this.midpointLineToMidpointDictionary = midpointLineToMidpointDictionary;
        }

        public Line GetMidpointLineFromTo(VoronoiSeedData from, VoronoiSeedData to)
        {
            Dictionary<VoronoiSeedData, Line> midpointLineDictionaryFrom = GetMidpointLineDictionaryFrom(from);

            if(!midpointLineDictionaryFrom.ContainsKey(to))
            {
                Debug.LogError("Dictionary for midpoint lines from seed " + from + "\n doesn't have a midpoint line for seed " + to);
                return null;
            }

            return midpointLineDictionaryFrom[to];
        }

        public bool TryGetMidpointLineFromTo(VoronoiSeedData from, VoronoiSeedData to, out Line line)
        {
            line = null;

            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>> midpointDictionary = GetMidpointLineFromToDictionary();
            bool containsFirstKey = midpointDictionary.ContainsKey(from);
            
            if(!containsFirstKey)
            {
                return false;
            }

            Dictionary<VoronoiSeedData, Line> midpointDictionaryFrom = GetMidpointLineDictionaryFrom(from);
            bool containsSecondKey = midpointDictionaryFrom.ContainsKey(to);

            if(!containsSecondKey)
            {
                return false;
            }

            line = GetMidpointLineFromTo(from, to);
            return true;
        }

        public Line[] GetAllMidpointLinesNotToOrFromSeed(VoronoiSeedData seedToOmit)
        {
            List<Line> linesNotFromSeed = new List<Line>();

            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>> midpointDictionary = GetMidpointLineFromToDictionary();

            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>>.KeyCollection upperLayerKeys = midpointDictionary.Keys;

            foreach (VoronoiSeedData endpoint1 in upperLayerKeys)
            {
                if(endpoint1.Equals(seedToOmit))
                {
                    continue;
                }

                Dictionary<VoronoiSeedData, Line> midpointDictionaryLowerLayer = GetMidpointLineDictionaryFrom(endpoint1);

                Dictionary<VoronoiSeedData, Line>.KeyCollection lowerLayerKeys = midpointDictionaryLowerLayer.Keys;

                foreach(VoronoiSeedData endpoint2 in lowerLayerKeys)
                {
                    if (endpoint2.Equals(seedToOmit))
                    {
                        continue;
                    }

                    Line midpointLine = GetMidpointLineFromTo(endpoint1, endpoint2);

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

        private Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>> GetMidpointLineFromToDictionary()
        {
            return this.midpointLineFromToDictionary;
        }

        private void SetMidpointLineFromToDictionary(Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>> midpointLineFromToDictionary)
        {
            this.midpointLineFromToDictionary = midpointLineFromToDictionary;
        }

        private void InitMidpointLineFromToDictionary()
        {
            SetMidpointLineFromToDictionary(new Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>>());
            VoronoiSeedData[] allSeeds = GetAllSeeds();

            for (int i = 0; i < allSeeds.Length; ++i)
            {
                VoronoiSeedData seed = allSeeds[i];
                InitMidpointLinesFromToForSingleSeed(seed);
            }
        }

        private void InitMidpointLinesFromToForSingleSeed(VoronoiSeedData seed)
        {
            Dictionary<VoronoiSeedData, Dictionary<VoronoiSeedData, Line>> midpointLinesFromTo = GetMidpointLineFromToDictionary();
            midpointLinesFromTo.Add(seed, new Dictionary<VoronoiSeedData, Line>());
        }

        public Dictionary<VoronoiSeedData, Line> GetMidpointLineDictionaryFrom(VoronoiSeedData seed)
        {
            return GetMidpointLineFromToDictionary()[seed];
        }

        private void AddMidpointLineFromTo(VoronoiSeedData from, VoronoiSeedData to, Line midpointLine)
        {
            GetMidpointLineDictionaryFrom(from).Add(to, midpointLine);
        }

        private void CalculateMidpointLinesForAllPoints()
        {
            InitMidpointLineFromToDictionary();

            // Init midpoint line to midpoint dictionary
            SetMidpointLineToMidpointDictionary(new Dictionary<Line, Vector2>());

            VoronoiSeedData[] allSeeds = GetAllSeeds();

            List<VoronoiSeedData> endpointLowerKeys = new List<VoronoiSeedData>();

            for(int i = 0; i < allSeeds.Length; ++i)
            {
                VoronoiSeedData seedToGenerateFor = allSeeds[i];

                /* Enumerate all but other but so we can easily access this struct */
                for(int j = 0; j < allSeeds.Length; ++j)
                {
                    // Do not construct a line from itself to itself
                    if(j == i)
                    {
                        continue;
                    }

                    VoronoiSeedData otherSeed = allSeeds[j];

                    /*
                     * The dictionary is built so you can access the same line when you go
                     * from: endpoint1
                     * to: endpoint2
                     * return: line12
                     * 
                     * and
                     * 
                     * from: endpoint2
                     * to: endpoint1
                     * return: line12
                     * 
                     * The order of the to and from parameter does not matter. This would work even without this piece
                     * of code, but the order of parameters would return EQUAL objects by value, but two different objects by reference.
                     * Thanks to this both parameter order scenarios will point to the same object.
                     */
                    if(TryGetMidpointLineFromTo(otherSeed, seedToGenerateFor, out Line existingLine))
                    {
                        AddMidpointLineFromTo(seedToGenerateFor, otherSeed, existingLine);
                        continue;
                    }


                    float otherX = otherSeed.GetX();
                    float otherY = otherSeed.GetY();

                    // A line segment starting at the center of this obj and ending at the center of the other seed.
                    LineSegment thisCenterToSeedCenterLine = new LineSegment(seedToGenerateFor.GetX(), seedToGenerateFor.GetY(), otherX, otherY);

                    Vector2 midpoint = thisCenterToSeedCenterLine.GetPointAt(0.5f);

                    // Get a line perpendicular to this line, going through a point half through this line segment.
                    Line midpointLine = thisCenterToSeedCenterLine.GetLinePerpendicularAtWay(0.5f);

                    AddMidpointLineFromTo(seedToGenerateFor, otherSeed, midpointLine);
                    AddMidpointLineToMidpointEntry(midpointLine, midpoint);
                }
            }
        }

        public Line[] GetAllMidpointLinesFrom(VoronoiSeedData seed)
        {
            Dictionary<VoronoiSeedData, Line> allLinesFromSeedToOtherDictionary = GetMidpointLineDictionaryFrom(seed);
            return allLinesFromSeedToOtherDictionary.Values.ToArray();
        }

        private void AddMidpointLineToMidpointEntry(Line midpointLine, Vector2 midpoint)
        {
            GetMidpointLineToMidpointDictionary().Add(midpointLine, midpoint);
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

