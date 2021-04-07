using System.Collections.Generic;
using UnityEngine;


namespace ElectedByVictory.WorldCreation
{
    public class FullVoronoiSeed
    {

        private GameObject rMeshObject;
        private VoronoiSeedProvinceInit voronoiSeedProvinceInit;

        private VoronoiSeedData voronoiSeedData;
        private SharedVoronoiWorldData sharedData;

        public FullVoronoiSeed(VoronoiSeedData voronoiSeedData)
        {
            SetVoronoiSeedDataUpdate(voronoiSeedData);
        }

        private void SetVoronoiSeedDataNonUpdate(VoronoiSeedData voronoiSeedData)
        {
            this.voronoiSeedData = voronoiSeedData;
        }

        private void SetVoronoiSeedDataUpdate(VoronoiSeedData voronoiSeedData)
        {
            SetVoronoiSeedDataNonUpdate(voronoiSeedData);
            TryUpdateMesh();
        }

        private void TryUpdateMesh()
        {
            if(IsReadyForMeshConstruction())
            {
                UpdateMeshObjectUnsafe();
            }
        }

        public void UpdateSharedVoronoiWorldDataUpdate(SharedVoronoiWorldData sharedData)
        {
            UpdateSharedVoronoiWorldDataNonUpdate(sharedData);
            TryUpdateMesh();
        }

        public void UpdateSharedVoronoiWorldDataNonUpdate(SharedVoronoiWorldData sharedData)
        {
            this.sharedData = sharedData;
        }

        public VoronoiSeedData GetVoronoiSeedData()
        {
            return this.voronoiSeedData;
        }

        private SharedVoronoiWorldData GetSharedData()
        {
            return this.sharedData;
        }

        private void SetMeshObject(GameObject rMeshObject)
        {
            this.rMeshObject = rMeshObject;
        }

        private GameObject GetMeshObject()
        {
            return this.rMeshObject;
        }

        public void InstantiateMeshObject()
        {
            // Clear the last mesh object
            if(HasMeshObject())
            {
                GameObject.Destroy(GetMeshObject());
                SetMeshObject(null);
            }

            GameObject rMeshObject = GameObject.Instantiate(GameResources.GET_INSTANCE().GetVoronoiMeshObject());
            
            SetMeshObject(rMeshObject);
            UpdateMeshObjectUnsafe();
        }

        public bool HasMeshObject()
        {
            return (GetMeshObject() != null);
        }

        /// <summary>
        /// Return true, if this object is fully setup and has all the data necessary to receive mesh
        /// calculation requests.
        /// </summary>
        /// <returns></returns>
        public bool IsReadyForCalculation()
        {
            return (GetSharedData() != null);
        }

        public bool IsReadyForMeshConstruction()
        {
            return (IsReadyForCalculation() && HasMeshObject());
        }

        /// <summary>
        /// 
        /// Documentation (version 1) - Last Update: 26.3.2021 - 11:52
        /// 
        /// Updates the vertices of the associated mesh objects.
        /// Marked as unsafe, since you can call this method on an <see cref="FullVoronoiSeed"/> object without
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
            rMeshObject.transform.position = GetVoronoiSeedData().GetPosition();

            VoronoiMeshCalculator thisCalculator = new VoronoiMeshCalculator(GetVoronoiSeedData(), GetSharedData());
            Vector2[] vertices = thisCalculator.GetVertices();
        }

    }

    public class VoronoiMeshCalculator
    {

        private VoronoiSeedData voronoiSeed;
        private SharedVoronoiWorldData worldData;

        public VoronoiMeshCalculator(VoronoiSeedData voronoiSeed, SharedVoronoiWorldData worldData)
        {
            SetVoronoiSeed(voronoiSeed);
            SetWorldData(worldData);
        }

        private static List<Line> FilterMidpointLinesToPerimeterLines(Line[] midpointLinesFromThisSeed)
        {

        }

        public Vector2[] GetVertices()
        {
            List<Vector2> vertices = new List<Vector2>();

            VoronoiSeedData thisSeed = GetVoronoiSeedData();
            SharedVoronoiWorldData worldData = GetWorldData();
            VoronoiSeedData[] allSeeds = worldData.GetAllSeeds();
            CornerData cornerData = worldData.GetCornerData();

            cornerData.TryPopulateVerticeListWithClosestCorners(allSeeds, thisSeed, vertices);

            Line[] midpointLinesFromThisSeed = worldData.GetAllMidpointLinesFrom(thisSeed);

            List<Line> voronoiCellPerimeterLines = new List<Line>();

            for(int i = 0; i < midpointLinesFromThisSeed.Length; ++i)
            {

            }


            /*
            VoronoiSeedData closestSeed = worldData.GetClosestSeedFromAllSeedsTo(thisSeed);

            Line midpointLineToClosestSeed = worldData.GetMidpointLineFromTo(thisSeed, closestSeed);
            Vector2 midpointToClosestSeed = worldData.GetMidpointFromMidpointLine(midpointLineToClosestSeed);

            Line distancePlaneSplitLine = midpointLineToClosestSeed.GetShallowPerpendicularLine();
            distancePlaneSplitLine.SetTravelThroughPoint(midpointToClosestSeed);


            Line[] otherMidpointLines = worldData.GetAllMidpointLinesNotToOrFromSeed(thisSeed);

            ILineAndSegmentUnion lineToClosestSeed = thisSeed.GetLineToSeed(closestSeed);

            // Combine the other midpoint lines and edge lines
            List <ILineAndSegmentUnion> aggregationList = new List<ILineAndSegmentUnion>();
            aggregationList.AddRange(otherMidpointLines);
            aggregationList.AddRange(cornerData.GetEdgeLines());
            aggregationList.Add(lineToClosestSeed);

            ILineAndSegmentUnion[] midpointAndEdgeLines = aggregationList.ToArray();

            AddFirstTwoPlaneIntersectionPoints(thisSeed, vertices, midpointLineToClosestSeed, midpointAndEdgeLines, lineToClosestSeed, midpointToClosestSeed);
            */
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
