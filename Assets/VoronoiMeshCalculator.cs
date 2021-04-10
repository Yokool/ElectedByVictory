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

            /*
            for(int i = 0; i < midpointLineUtilsFromThisSeed.Length; ++i)
            {
                MidpointLineUtil filteringLineUtil = midpointLineUtilsFromThisSeed[i];
                Line filteringLine = filteringLineUtil.GetLine();

                // Not getting a value for the plane assignment would be very exceptional and you
                // would have to look into it.
                HalfPlane thisSeedAssignedHalfPlane = filteringLine.GetLinePlaneAssignment(thisSeed).Value;

                for(int j = 0; j < midpointLineUtilsFromThisSeed.Length; ++j)
                {
                    // Ignore the filtering line
                    if(j == i)
                    {
                        continue;
                    }

                    MidpointLineUtil otherLine = midpointLineUtils[j];
                    Vector2 otherLineMidpoint = otherLine.GetMidpoint();

                    HalfPlane? otherSeedHalfPlane = filteringLine.GetLinePlaneAssignment(otherLineMidpoint);

                    // If the otherLineMidpoint is directly on this line, then it's okay. Since it belongs to both planes.
                    // Otherwise check if the midpoint line is in the same halfplane as thisSeed.
                    if(!otherSeedHalfPlane.HasValue || thisSeedAssignedHalfPlane.Equals(otherSeedHalfPlane))
                    {
                        // Success, don't filter this line.
                        continue;
                    }

                    // Failure, get rid of this line.

                }

            }
            */

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

            cornerData.TryPopulateVerticeListWithClosestCorners(allSeeds, thisSeed, vertices);

            Debug.Log(vertices.Count);

            MidpointLineUtil[] midpointLineUtilsFromThisSeed = worldData.GetAllMidpointLineUtilsFrom(thisSeed);

            Debug.Log(midpointLineUtilsFromThisSeed.Length);

            Vector2 thisSeedPosition = thisSeed.GetPosition();

            // TODO: THIS FILTERING DOESN'T WORK, LOOK INTO YOUR PICTURES

            MidpointLineUtil[] perimeterMidpointLineUtils = FilterMidpointLinesFromSeedToPerimeterLines(thisSeedPosition, midpointLineUtilsFromThisSeed);
            Line[] perimeterMidpointLines = MidpointLineUtilStatic.ExtractLines(perimeterMidpointLineUtils);

            Debug.Log(perimeterMidpointLines.Length);

            // All lines contains the perimeter lines and the cornerEdgeLines that close the world.
            List<ILineAndSegmentUnion> allLines = new List<ILineAndSegmentUnion>();
            allLines.AddRange(perimeterMidpointLines);
            allLines.AddRange(cornerEdgeLines);

            // TODO: YOU CAN GET TWO INTERSECTIONS ON THE SAME HALFPLANE
            // TODO: YOU CAN GET TWO INTERSECTIONS ON THE SAME HALFPLANE
            // TODO: YOU CAN GET TWO INTERSECTIONS ON THE SAME HALFPLANE
            // TODO: YOU CAN GET TWO INTERSECTIONS ON THE SAME HALFPLANE

            /*
             * How to tell if two seeds share a border? 
             * 
             * It is NOT the distance between the seeds.
             * 
             * The set of perimeter points which are not the vertices of the
             * mesh make up the sides of the voronoi cells. Lets call this set PS.
             * 
             * If X is a member of PS then:
             * 
             * Distance from this seed center to X is the equal as the distance of X to ONE other seed center.
             * 
             * The vertices of the perimeter (which are not created by the intersection with the world edge) are where:
             * 
             * If vertex is V, then the distance from V to this seed center
             * is equal to MORE THAN ONE other seed.
             * 
            */

            /*
             * For every midpoint line: 
             * 
             * Get the TWO closest intersections on each Half_Plane. The intersections are checked against
             * other midpoint lines and the edge lines.
             * 
             * It is possible that you will only get ONE point on the half plane, in that case you are intersecting
             * the edge of the world.
             * 
             * For each half plane:
             * 
             * Create a line segment from the first closest vertex to the second.
             * 
             * If you only have 1 closest vertex then the first vertex will be the midpoint
             * inbetween the seeds.
             * 
             * For these line segments get the midpoint for each one.
             * 
             * NOW YOU CAN FINALLY USE THESE LINES AND MIDPOINTS AS INTENDED
             * IN THE FILTERING METHOD.
             * 
            */

            // Although we want to find intersection only for the perimeterLines with all the lines (this means including
            // the edge lines for voronoi cells that are hugging the edge of the world)
            for (int i = 0; i < perimeterMidpointLineUtils.Length; ++i)
            {
                MidpointLineUtil perimeterMidpointLineUtil = perimeterMidpointLineUtils[i];
                Line perimeterMidpointLine = perimeterMidpointLineUtil.GetLine();
                Vector2 perimeterLineMidpoint = perimeterMidpointLineUtil.GetMidpoint();

                Line perpendicularPlaneAssignmentLine = perimeterMidpointLine.GetDeepPerpendicularLinePoint(perimeterLineMidpoint);
                ClosestHalfPlanePoints closestHalfPlanePoints = new ClosestHalfPlanePoints(perimeterLineMidpoint, perpendicularPlaneAssignmentLine);
                
                for(int j = 0; j < allLines.Count; ++j)
                {
                    ILineAndSegmentUnion otherLine = allLines[j];

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

                (HalfPlane, Vector2?)[] closestIntersectionPointsOnHalfplanes = closestHalfPlanePoints.GetAllPlanesAndClosestPoints();

                for(int k = 0; k < closestIntersectionPointsOnHalfplanes.Length; ++k)
                {
                    // not getting at least one intersection point on the halfplane would be very exceptional
                    Vector2? intersectionPoint = closestIntersectionPointsOnHalfplanes[k].Item2;

                    if(!intersectionPoint.HasValue)
                    {
                        //Debug.LogError("Exceptional situation.");
                        continue;
                    }

                    vertices.Add(intersectionPoint.Value);

                }

            }

            Vector2[] sortedByAngleVertices = VertexMath.SortVerticesByAngleToPoint(vertices.ToArray(), thisSeedPosition);

            Debug.Log(sortedByAngleVertices.Length);

            Debug.Log("END OF CELL");

            return vertices.ToArray();
        }

        private static void AddFirstTwoPlaneIntersectionPoints(VoronoiSeedData thisSeed, List<Vector2> vertices, Line midpointLineToClosestSeed,
            ILineAndSegmentUnion[] midpointAndEdgeLines, IPointPlaneAssignment lineToClosestSeed, Vector2 midpointToClosestSeed)
        {
            ClosestHalfPlanePoints closestPointsObj = FindClosestIntersectionsFromLineForLinesOnSeperatePlanes(midpointLineToClosestSeed, midpointAndEdgeLines, lineToClosestSeed, midpointToClosestSeed);

            (HalfPlane, Vector2?)[] planesAndClosestPoints = closestPointsObj.GetAllPlanesAndClosestPoints();

            for (int i = 0; i < planesAndClosestPoints.Length; ++i)
            {
                (HalfPlane, Vector2?) planeAndClosestPoint = planesAndClosestPoints[i];

                HalfPlane plane = planeAndClosestPoint.Item1;

                // We should always be able to find at least one intersection
                // But actually there is a chance that the intersection point will be on the line
                // TODO: LOOK FOR ERRORS, IF THIS HAPPENS

                if (!planeAndClosestPoint.Item2.HasValue)
                {
                    Debug.LogError("See comment.");
                    continue;
                }

                Vector2 closestIntersectionPoint = planeAndClosestPoint.Item2.Value;

                vertices.Add(closestIntersectionPoint);

                Line newPerpendicularLine = midpointLineToClosestSeed.GetShallowPerpendicularLine();
                newPerpendicularLine.SetTravelThroughPoint(closestIntersectionPoint);

                //^^Use this line for the new intersection
                // Basically recreate this method but instead of adding two points, add only a single point
                // And that is the point that is closest to the thisSeed
            }
        }

        /// <summary>
        /// Finds an the closest intersections for all existing <see cref="Plane"/>s of a <see cref="Line"/> with a list of <see cref="ILineAndSegmentUnion"/>
        /// </summary>
        private static ClosestHalfPlanePoints FindClosestIntersectionsFromLineForLinesOnSeperatePlanes(Line line, ILineAndSegmentUnion[] possibleIntersectionLines,
            IPointPlaneAssignment planeAssignmentLine, Vector2 pointToCalculateDistanceTo)
        {
            ClosestHalfPlanePoints closestIntersectionPointsForPlanes = new ClosestHalfPlanePoints(pointToCalculateDistanceTo, planeAssignmentLine);
            for (int i = 0; i < possibleIntersectionLines.Length; ++i)
            {
                ILineAndSegmentUnion otherLine = possibleIntersectionLines[i];

                Vector2? intersection = otherLine.GetIntersectionWithLine(line);

                if (intersection.HasValue)
                {
                    closestIntersectionPointsForPlanes.TrySetPointAsClosest(intersection.Value);
                }
            }
            return closestIntersectionPointsForPlanes;
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
