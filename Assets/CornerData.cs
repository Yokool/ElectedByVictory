using System;
using System.Collections.Generic;
using UnityEngine;


namespace ElectedByVictory.WorldCreation
{
    public struct CornerData
    {
        private Vector2 corner00;
        private Vector2 corner10;
        private Vector2 corner01;
        private Vector2 corner11;

        public CornerData(Vector2 corner00, Vector2 corner10, Vector2 corner01, Vector2 corner11) : this()
        {
            SetCorners(corner00, corner10, corner01, corner11);
        }

        public LineSegment[] GetEdgeLines()
        {
            LineSegment[] edgeLines = new LineSegment[4];

            // Bottom edge
            edgeLines[0] = new LineSegment(GetCorner00(), GetCorner10());

            // Left edge
            edgeLines[1] = new LineSegment(GetCorner00(), GetCorner01());

            // Top edge
            edgeLines[2] = new LineSegment(GetCorner01(), GetCorner11());

            // Right edge
            edgeLines[3] = new LineSegment(GetCorner11(), GetCorner10());

            return edgeLines;
        }

        public void SetCorners(Vector2 corner00, Vector2 corner10, Vector2 corner01, Vector2 corner11)
        {
            SetCorner00(corner00);
            SetCorner01(corner01);
            SetCorner10(corner10);
            SetCorner11(corner11);
        }

        public void SetupCornersFromSquare(Vector2 center, float width, float height)
        {
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            SetCorner00(center + new Vector2(-halfWidth, -halfHeight));
            SetCorner01(center + new Vector2(-halfWidth, halfHeight));
            SetCorner10(center + new Vector2(halfWidth, -halfHeight));
            SetCorner11(center + new Vector2(halfWidth, halfHeight));
        }

        public VoronoiSeedData GetSeedClosestToCorner00(VoronoiSeedData[] voronoiSeeds)
        {
            return internalGetClosestSeedToCorner(GetCorner00(), voronoiSeeds);
        }

        public VoronoiSeedData GetSeedClosestToCorner10(VoronoiSeedData[] voronoiSeeds)
        {
            return internalGetClosestSeedToCorner(GetCorner10(), voronoiSeeds);
        }

        public VoronoiSeedData GetSeedClosestToCorner01(VoronoiSeedData[] voronoiSeeds)
        {
            return internalGetClosestSeedToCorner(GetCorner01(), voronoiSeeds);
        }

        public VoronoiSeedData GetSeedClosestToCorner11(VoronoiSeedData[] voronoiSeeds)
        {
            return internalGetClosestSeedToCorner(GetCorner11(), voronoiSeeds);
        }

        public bool IsSeedClosestToCorner00(VoronoiSeedData[] allSeeds, VoronoiSeedData seed)
        {
            return internalIsSeedClosestToCorner(GetCorner00(), allSeeds, seed);
        }

        public bool IsSeedClosestToCorner10(VoronoiSeedData[] allSeeds, VoronoiSeedData seed)
        {
            return internalIsSeedClosestToCorner(GetCorner10(), allSeeds, seed);
        }

        public bool IsSeedClosestToCorner01(VoronoiSeedData[] allSeeds, VoronoiSeedData seed)
        {
            return internalIsSeedClosestToCorner(GetCorner01(), allSeeds, seed);
        }

        public bool IsSeedClosestToCorner11(VoronoiSeedData[] allSeeds, VoronoiSeedData seed)
        {
            return internalIsSeedClosestToCorner(GetCorner11(), allSeeds, seed);
        }

        public bool TryGetCornerIfClosestTo00(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, out Vector2 outputCorner)
        {
            return internalTryGetCornerIfClosestTo(allSeeds, seed, GetCorner00(), out outputCorner);
        }

        public bool TryGetCornerIfClosestTo10(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, out Vector2 outputCorner)
        {
            return internalTryGetCornerIfClosestTo(allSeeds, seed, GetCorner10(), out outputCorner);
        }

        public bool TryGetCornerIfClosestTo01(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, out Vector2 outputCorner)
        {
            return internalTryGetCornerIfClosestTo(allSeeds, seed, GetCorner01(), out outputCorner);
        }

        public bool TryGetCornerIfClosestTo11(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, out Vector2 outputCorner)
        {
            return internalTryGetCornerIfClosestTo(allSeeds, seed, GetCorner11(), out outputCorner);
        }

        public void TryPopulateVerticeListWithClosestCorners(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, List<Vector2> vertices)
        {
            TryPopulateListWithCorner00(allSeeds, seed, vertices);
            TryPopulateListWithCorner10(allSeeds, seed, vertices);
            TryPopulateListWithCorner01(allSeeds, seed, vertices);
            TryPopulateListWithCorner11(allSeeds, seed, vertices);
        }

        public void TryPopulateListWithCorner00(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, List<Vector2> vertices)
        {
            internalTryPopulateListWithCorner(allSeeds, seed, GetCorner00(), vertices);
        }

        public void TryPopulateListWithCorner10(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, List<Vector2> vertices)
        {
            internalTryPopulateListWithCorner(allSeeds, seed, GetCorner10(), vertices);
        }

        public void TryPopulateListWithCorner01(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, List<Vector2> vertices)
        {
            internalTryPopulateListWithCorner(allSeeds, seed, GetCorner01(), vertices);
        }

        public void TryPopulateListWithCorner11(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, List<Vector2> vertices)
        {
            internalTryPopulateListWithCorner(allSeeds, seed, GetCorner11(), vertices);
        }

        private static void internalTryPopulateListWithCorner(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, Vector2 corner, List<Vector2> vertices)
        {
            if(internalTryGetCornerIfClosestTo(allSeeds, seed, corner, out Vector2 outputCorner))
            {
                vertices.Add(outputCorner);
            }
        }

        private static bool internalTryGetCornerIfClosestTo(VoronoiSeedData[] allSeeds, VoronoiSeedData seed, Vector2 corner, out Vector2 outputCorner)
        {
            bool success = internalIsSeedClosestToCorner(corner, allSeeds, seed);
            outputCorner = success ? corner : Vector2.zero;
            return success;
        }

        private static bool internalIsSeedClosestToCorner(Vector2 corner, VoronoiSeedData[] allSeeds, VoronoiSeedData seed)
        {
            return internalGetClosestSeedToCorner(corner, allSeeds).Equals(seed);
        }

        private static VoronoiSeedData internalGetClosestSeedToCorner(Vector2 corner, VoronoiSeedData[] voronoiSeeds)
        {

            if(voronoiSeeds.Length == 0)
            {
                throw new ArgumentException($"{nameof(internalGetClosestSeedToCorner)} can not receive the seed list with the length of 0.");
            }

            float cornerX = corner.x;
            float cornerY = corner.y;

            int smallestDistanceIndex = -1;

            float smallestDistance = float.MaxValue;

            for(int i = 0; i < voronoiSeeds.Length; ++i)
            {
                VoronoiSeedData seed = voronoiSeeds[i];
                float seedX = seed.GetX();
                float seedY = seed.GetY();

                float distance = MathEBV.PointDistance(seedX, seedY, cornerX, cornerY);

                if(distance < smallestDistance)
                {
                    smallestDistance = distance;
                    smallestDistanceIndex = i;
                }

            }

            return voronoiSeeds[smallestDistanceIndex];

        }

        private void SetCorner00(Vector2 Corner00)
        {
            this.corner00 = Corner00;
        }

        private void SetCorner10(Vector2 Corner10)
        {
            this.corner10 = Corner10;
        }

        private void SetCorner01(Vector2 Corner01)
        {
            this.corner01 = Corner01;
        }

        private void SetCorner11(Vector2 Corner11)
        {
            this.corner11 = Corner11;
        }

        public Vector2 GetCorner00()
        {
            return this.corner00;
        }

        public Vector2 GetCorner10()
        {
            return this.corner10;
        }

        public Vector2 GetCorner01()
        {
            return this.corner01;
        }

        public Vector2 GetCorner11()
        {
            return this.corner11;
        }
    }

}

