using System;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    public struct VoronoiSeedData
    {

        public override string ToString()
        {
            return $"[{nameof(VoronoiSeedData)}: x = {GetX()} _ y = {GetY()}]";
        }

        public Circle REMOVETHISMETHOD_DO_NOT_CALL_GetCircle()
        {
            throw new Exception("DO NOT CALL");
        }

        /*
        public override bool Equals(object obj)
        {
            if(!(obj is VoronoiSeedData))
            {
                return false;
            }

            VoronoiSeedData objSeed = (VoronoiSeedData)obj;

            bool xEquals = MathEBV.FloatEquals(GetX(), objSeed.GetX());
            bool yEquals = MathEBV.FloatEquals(GetY(), objSeed.GetY());
            
            return (xEquals && yEquals);

        }

        public override int GetHashCode()
        {
            int hash = 65;
            hash = (4 * hash) + GetX().GetHashCode();
            hash = (4 * hash) + GetY().GetHashCode();

            return hash;

        }
        */


        /// <summary>
        /// The x world position of the seed.
        /// </summary>
        private float x;
        /// <summary>
        /// The y world position of the seed.
        /// </summary>
        private float y;

        private Circle circleEquation;

        public VoronoiSeedData(float x, float y) : this()
        {
            SetX(x);
            SetY(y);
        }

        public VoronoiSeedData(Vector2 seedCenter) : this(seedCenter.x, seedCenter.y)
        {

        }

        public void SetX(float x)
        {
            this.x = x;
            circleEquation.SetX(x);
        }

        public void SetY(float y)
        {
            this.y = y;
            circleEquation.SetY(y);
        }

        public float GetX()
        {
            return this.x;
        }

        public float GetY()
        {
            return this.y;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(GetX(), GetY());
        }

        public LineSegment GetLineToSeed(VoronoiSeedData seed)
        {
            return new LineSegment(this.GetPosition(), seed.GetPosition());
        }

    }

}

