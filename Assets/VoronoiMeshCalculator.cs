using System.Collections.Generic;
using UnityEngine;


namespace ElectedByVictory.WorldCreation
{
    public class VoronoiMeshCalculator
    {

        private VoronoiSeedData voronoiSeed;
        private SharedVoronoiWorldData worldData;

        public VoronoiMeshCalculator(VoronoiSeedData voronoiSeed, SharedVoronoiWorldData worldData)
        {
            SetVoronoiSeed(voronoiSeed);
            SetWorldData(worldData);
        }

        private static MidpointLineUtil[] FilterMidpointLinesFromSeedToPerimeterLines(Vector2 thisSeedPosition, MidpointLineUtil[] midpointLineUtilsFromThisSeed)
        {
            List<MidpointLineUtil> perimeterLineUtils = new List<MidpointLineUtil>();

            for(int i = 0; i < midpointLineUtilsFromThisSeed.Length; ++i)
            {
                MidpointLineUtil lineUtilToCheck = midpointLineUtilsFromThisSeed[i];
                Vector2 midpointToCheck = lineUtilToCheck.GetMidpoint();


                bool success = true;

                for(int j = 0; j < midpointLineUtilsFromThisSeed.Length; ++j)
                {
                    if(i == j)
                    {
                        continue;
                    }

                    MidpointLineUtil checkingLineUtil = midpointLineUtilsFromThisSeed[j];
                    Line checkingLine = checkingLineUtil.GetLine();

                    // If you were to take the line segments of each side of the voronoi cell and transform them into a line
                    // They should never intersect the center as the voronoi shapes are convex
                    // Therefore you can get .Value without checking if it .HasValue
                    HalfPlane thisSeedCheckLineAssignment = checkingLine.GetLinePlaneAssignment(thisSeedPosition).Value;


                    HalfPlane? midpointCheckLineAssignment = checkingLine.GetLinePlaneAssignment(midpointToCheck);

                    

                    Debug.Log("Checking line point: " + checkingLineUtil.GetMidpoint());
                    Debug.Log("thisSeedPosition: " + thisSeedPosition);
                    Debug.Log("midpointPosition: " + midpointToCheck);

                    //Debug.Log("plane1 " + thisSeedCheckLineAssignment + " plane2 " + midpointCheckLineAssignment);

                    // If it doesn't have a value then it's exactly on the checking line => this is ok.
                    // The point still belongs to both half planes => also means to the plane where the voronoi cell center is.
                    if (!midpointCheckLineAssignment.HasValue || thisSeedCheckLineAssignment.Equals(midpointCheckLineAssignment))
                    {
                        Debug.Log("success");
                        // Success for THIS checkingLine, continue onto the next.
                        // Remember we're checking for failures => success means move onto the next checkingLine.
                        // Success on all lines means add this line later in the code.
                        continue;
                    }
                    else
                    {
                        Debug.Log("failure");
                        // If we catch at least one filtering line that is closer to the center then disregard the
                        // line we ar checking for.
                        success = false;
                        break;
                    }

                }

                //Debug.Log(success);

                // Ultimate success
                if(success)
                {
                    perimeterLineUtils.Add(lineUtilToCheck);
                }
            }

            return perimeterLineUtils.ToArray();


        }

        public Vector2[] GetVertices()
        {

            Debug.Log("NEW CELL");

            List<Vector2> vertices = new List<Vector2>();

            VoronoiSeedData thisSeed = GetVoronoiSeedData();
            SharedVoronoiWorldData worldData = GetWorldData();
            VoronoiSeedData[] allSeeds = worldData.GetAllSeeds();
            CornerData cornerData = worldData.GetCornerData();
            LineSegment[] cornerEdgeLines = cornerData.GetEdgeLines();
            Vector2 thisSeedPosition = thisSeed.GetPosition();

            cornerData.TryPopulateVerticeListWithClosestCorners(allSeeds, thisSeed, vertices);

            Debug.Log(vertices.Count);

            MidpointLineUtil[] perimeterMidpointLineUtils = worldData.GetAllMidpointLineUtilsFrom(thisSeed);

            Debug.Log(perimeterMidpointLineUtils.Length);


            Line[] perimeterMidpointLines = MidpointLineUtilStatic.ExtractLines(perimeterMidpointLineUtils);

            // All lines contains the perimeter lines and the cornerEdgeLines that close the world.
            List<ILineRaySegmentUnion> allLines = new List<ILineRaySegmentUnion>();
            allLines.AddRange(perimeterMidpointLines);
            allLines.AddRange(cornerEdgeLines);


            List<LineSegment> adjustedMidpointLines = new List<LineSegment>(perimeterMidpointLineUtils.Length);

            // Searching for the two closest points:
            for (int i = 0; i < perimeterMidpointLineUtils.Length; ++i)
            {
                MidpointLineUtil perimeterMidpointLineUtil = perimeterMidpointLineUtils[i];
                Line perimeterMidpointLine = perimeterMidpointLineUtil.GetLine();
                Vector2 perimeterLineMidpoint = perimeterMidpointLineUtil.GetMidpoint();

                Line perpendicularPlaneAssignmentLine = perimeterMidpointLine.GetDeepPerpendicularLinePoint(perimeterLineMidpoint);
                ClosestHalfPlanePoints closestHalfPlanePoints = new ClosestHalfPlanePoints(perimeterLineMidpoint, perpendicularPlaneAssignmentLine, 2);
                
                for(int j = 0; j < allLines.Count; ++j)
                {
                    ILineRaySegmentUnion otherLine = allLines[j];

                    // allLines contains the line we are finding the intersections for - obviously
                    // we don't want an intersection with ourselves so ignore it (checked by reference)
                    if(otherLine == perimeterMidpointLine)
                    {
                        continue;
                    }

                    Vector2? intersectionPoint = perimeterMidpointLine.GetIntersectionWithLineAndSegmentUnion(otherLine);

                    if(!intersectionPoint.HasValue)
                    {
                        continue;
                    }

                    closestHalfPlanePoints.TrySetPointAsClosest(intersectionPoint.Value);
                }

                // We should always at least hit the border of the voronoi diagram world.
                if (!closestHalfPlanePoints.HasEveryPlaneAtLeastOnePoint())
                {
                    Debug.LogError("This code block should not be executed.");
                    continue;
                }

                // If each plane has only a single point.
                if(closestHalfPlanePoints.NoPlaneHasMoreThanParameterPoints(1))
                {
                    HalfPlane[] planes = HalfPlaneUtilities.GetAllPlanes();

                    Vector2[] segmentPoints = new Vector2[2]; 

                    for(int l = 0; l < segmentPoints.Length; ++l)
                    {
                        HalfPlane plane = planes[l];
                        
                        // ClosestPointForPlane should not be null here, as the planes will have at least one point.
                        segmentPoints[l] = closestHalfPlanePoints.GetClosestPointForPlane(plane).Value;
                    }

                    LineSegment adjustedLine = new LineSegment(segmentPoints[0], segmentPoints[1]);
                    adjustedMidpointLines.Add(adjustedLine);
                    continue;
                }

                // Here we are if any plane has more than 1 point.

                HalfPlane[] allPlanes = HalfPlaneUtilities.GetAllPlanes();

                for(int k = 0; k < allPlanes.Length; ++k)
                {
                    HalfPlane plane = allPlanes[k];

                    int nonNullPoints = closestHalfPlanePoints.NonNullPointsOnPlaneCount(plane);

                    LineSegment adjustedMidpointLine = null;

                    Vector2 closestPointOnPlane = closestHalfPlanePoints.GetClosestPointForPlane(plane).Value;

                    if (nonNullPoints == 2)
                    {
                        Vector2 furthestPoint = closestHalfPlanePoints.GetFurthestPointForPlane(plane).Value;
                        adjustedMidpointLine = new LineSegment(closestPointOnPlane, furthestPoint);
                    }
                    else if(nonNullPoints == 1)
                    {
                        adjustedMidpointLine = new LineSegment(perimeterLineMidpoint, closestPointOnPlane);
                    }

                    adjustedMidpointLines.Add(adjustedMidpointLine);

                }
            }

            Vector2[] sortedByAngleVertices = VertexMath.SortVerticesByAngleToPoint(vertices.ToArray(), thisSeedPosition);

            Debug.Log(sortedByAngleVertices.Length);

            Debug.Log("END OF CELL");

            return vertices.ToArray();
        }

        private void SetWorldData(SharedVoronoiWorldData worldData)
        {
            this.worldData = worldData;
        }

        private SharedVoronoiWorldData GetWorldData()
        {
            return this.worldData;
        }

        private void SetVoronoiSeed(VoronoiSeedData voronoiSeed)
        {
            this.voronoiSeed = voronoiSeed;
        }

        private VoronoiSeedData GetVoronoiSeedData()
        {
            return this.voronoiSeed;
        }
        /*
        public Line[] CreateClampingLines()
        {
            SharedVoronoiWorldData worldData = GetWorldData();

            // Omit out our seed, so -1 - we don't need a perpendicular line with ourselves.
            Line[] perpendicularLines = new Line[worldData.GetAllSeeds().Length - 1];

            VoronoiSeedData thisSeedData = GetVoronoiSeedData();

            float originX = thisSeedData.GetX();
            float originY = thisSeedData.GetY();

            int i = 0;

            SeedConsumer createClampingLine = (otherSeed) => {
                float seedCenterX = otherSeed.GetX();
                float seedCenterY = otherSeed.GetY();

                // A line segment starting at the center of this obj and ending at the center of the other seed.
                LineSegment thisCenterToSeedCenterLine = new LineSegment(originX, originY, seedCenterX, seedCenterY);

                // Get a line perpendicular to this line, going through a point half through this line segment.
                perpendicularLines[i] = thisCenterToSeedCenterLine.GetLinePerpendicularAtWay(0.5f);
                ++i;
            };

            worldData.EnumerateAllButSeed(thisSeedData, createClampingLine);

            return perpendicularLines;
        }
        */
    }

}
