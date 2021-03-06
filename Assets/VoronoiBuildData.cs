using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    public class VoronoiBuildData
    {
        private VoronoiSeedManager[] allSeedsCache;
        private VoronoiSeedManager seedOwner;

        private CornerData cornerDataCache;

        public VoronoiBuildData(VoronoiSeedManager seedOwner, VoronoiSeedManager[] allSeeds, CornerData cornerData)
        {
            SetAllSeedsCache(allSeeds);
            SetSeedOwner(seedOwner);
            SetCornerDataCache(cornerData);
        }

        private VoronoiSeedManager[] GetAllSeedsCache()
        {
            return this.allSeedsCache;
        }

        private VoronoiSeedManager GetSeedOwner()
        {
            return this.seedOwner;
        }

        private CornerData GetCornerDataCache()
        {
            return this.cornerDataCache;
        }

        private void SetCornerDataCache(CornerData cornerDataCache)
        {
            this.cornerDataCache = cornerDataCache;
        }

        private void SetAllSeedsCache(VoronoiSeedManager[] allSeedsCache)
        {
            this.allSeedsCache = allSeedsCache;
        }

        private void SetSeedOwner(VoronoiSeedManager seedOwner)
        {
            this.seedOwner = seedOwner;
        }

        private Vector2 GetVoronoiCellCenter()
        {
            Vector2 voronoiCellCenter = GetSeedOwner().GetVoronoiSeedData().GetPosition();
            return voronoiCellCenter;
        }

        public Vector2[] GetVoronoiCellVertices()
        {
            ILineRaySegmentUnion[] cellEdges = GetVoronoiCellEdges();


            Vector2[] cellVertices = LineUtilities.GetAllUniqueIntersections(cellEdges);

            if(cellEdges.Length != cellVertices.Length)
            {
                //Debug.Log(cellEdges.Length + " " + cellVertices.Length);
            }

            cellVertices = PointUtilities.SortPointsByAngleToPointToCopy(GetVoronoiCellCenter(), cellVertices);

            return cellVertices;
        }

        public ILineRaySegmentUnion[] GetVoronoiCellEdges()
        {
            ILineRaySegmentUnion[] allWorldLines = BuildAllLines();

            allWorldLines = CutOffToVoronoiEdges(allWorldLines);
            return allWorldLines;
        }

        private ILineRaySegmentUnion[] CutOffToVoronoiEdges(ILineRaySegmentUnion[] __allWorldLines)
        {
            ILineRaySegmentUnion[] allWorldLines_Copy = __allWorldLines.ToArray();
            Vector2 voronoiCellCenter = GetVoronoiCellCenter();

            for(int i = 0; i < allWorldLines_Copy.Length; ++i)
            {
                ILineRaySegmentUnion worldSegmentationLine = allWorldLines_Copy[i];

                if(worldSegmentationLine == null)
                {
                    continue;
                }    

                for(int j = 0; j < allWorldLines_Copy.Length; ++j)
                {
                    if(j == i)
                    {
                        continue;
                    }

                    ILineRaySegmentUnion lineToCut = allWorldLines_Copy[j];
                    
                    if(lineToCut == null)
                    {
                        continue;
                    }
                    
                    allWorldLines_Copy[j] = worldSegmentationLine.CutOff(voronoiCellCenter, lineToCut);
                }
            }

            return allWorldLines_Copy.Where( (lineUnion) => { return lineUnion != null; } ).ToArray();
        }

        private ILineRaySegmentUnion[] BuildAllLines()
        {
            LineSegment[] edgeLines = BuildEdgeLines();
            Line[] midpointLines = BuildMidpointLines();

            List<ILineRaySegmentUnion> allLines = new List<ILineRaySegmentUnion>();
            allLines.AddRange(edgeLines);
            allLines.AddRange(midpointLines);

            return allLines.ToArray();
        }

        private LineSegment[] BuildEdgeLines()
        {
            return GetCornerDataCache().GetEdgeLines();
        }

        private Line[] BuildMidpointLines()
        {
            VoronoiSeedManager seedOwner = GetSeedOwner();
            VoronoiSeedData seedOwnerData = seedOwner.GetVoronoiSeedData();
            Vector2 seedOwnerPosition = seedOwnerData.GetPosition();
            
            VoronoiSeedManager[] allSeeds = GetAllSeedsCache();

            // A line to all seeds but not this one.
            Line[] midpointLines = new Line[allSeeds.Length - 1];

            int indexWithout = 0;
            for (int i = 0; i < allSeeds.Length; ++i)
            {
                VoronoiSeedManager otherSeed = allSeeds[i];

                if(otherSeed == seedOwner)
                {
                    continue;
                }

                VoronoiSeedData otherSeedData = otherSeed.GetVoronoiSeedData();
                Vector2 otherSeedPosition = otherSeedData.GetPosition();

                LineSegment lineToOtherSeed = new LineSegment(seedOwnerPosition, otherSeedPosition);
                Line midpointLine = lineToOtherSeed.GetMidpointLine();

                midpointLines[indexWithout] = midpointLine;
                ++indexWithout;
            }

            return midpointLines;
        }

    }
}

