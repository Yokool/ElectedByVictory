using System;
using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    public struct VoronoiSeedData
    {

        public override string ToString()
        {
            return $"[{nameof(VoronoiSeedData)}: x = {GetX()} _ y = {GetY()} _ circle = {GetCircle()}]";
        }

        public override bool Equals(object obj)
        {
            if(!(obj is VoronoiSeedData))
            {
                return false;
            }

            VoronoiSeedData objSeed = (VoronoiSeedData)obj;

            bool xEquals = MathEBV.FloatEquals(GetX(), objSeed.GetX());
            bool yEquals = MathEBV.FloatEquals(GetY(), objSeed.GetY());
            bool circleEquals = GetCircle().Equals(objSeed.GetCircle());

            return xEquals && yEquals && circleEquals;

        }

        public override int GetHashCode()
        {
            int result = 0;

            int x = BitConverter.ToInt32(BitConverter.GetBytes(GetX()), 0);
            int y = BitConverter.ToInt32(BitConverter.GetBytes(GetX()), 0);

            int circleHashCode = GetCircle().GetHashCode();

            result = result * 31 + x;
            result = result * 31 + y;
            result = result * 31 + circleHashCode;

            return result;

        }

        /// <summary>
        /// The x world position of the seed.
        /// </summary>
        private float x;
        /// <summary>
        /// The y world position of the seed.
        /// </summary>
        private float y;

        private Circle circleEquation;

        public VoronoiSeedData(float x, float y, float circleRadius) : this()
        {
            SetX(x);
            SetY(y);
            SetCircleEquation(circleRadius);
        }

        public void SetCircleEquation(float circleRadius)
        {
            this.circleEquation = new Circle(GetX(), GetY(), circleRadius);
        }

        public Circle GetCircle()
        {
            return this.circleEquation;
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

