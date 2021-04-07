using System;
using UnityEngine;


namespace ElectedByVictory.WorldCreation
{
    public struct Circle
    {

        public override string ToString()
        {
            return $"[{nameof(Circle)} - x = {GetX()} _ y = {GetY()} _ radius = {GetRadius()}]";
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Circle))
            {
                return false;
            }

            Circle objCircle = (Circle)obj;

            bool xEquals = MathEBV.FloatEquals(GetX(), objCircle.GetX());
            bool yEquals = MathEBV.FloatEquals(GetY(), objCircle.GetY());
            bool radiusEquals = MathEBV.FloatEquals(GetRadius(), objCircle.GetRadius());

            return xEquals && yEquals && radiusEquals;

        }

        public override int GetHashCode()
        {
            int result = 0;
            
            int x = BitConverter.ToInt32(BitConverter.GetBytes(GetX()), 0);
            int y = BitConverter.ToInt32(BitConverter.GetBytes(GetY()), 0);
            int r = BitConverter.ToInt32(BitConverter.GetBytes(GetRadius()), 0);

            result = result * 31 + x;
            result = result * 31 + y;
            result = result * 31 + r;

            return result;

        }

        private float x;
        private float y;
        private float radius;
        
        public Circle(float x, float y, float radius) : this()
        {
            SetX(x);
            SetY(y);
            SetRadius(radius);
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        public void SetX(float x)
        {
            this.x = x;
        }

        public void SetY(float y)
        {
            this.y = y;
        }

        public float GetX()
        {
            return this.x;
        }

        public float GetY()
        {
            return this.y;
        }

        public float GetRadius()
        {
            return this.radius;
        }

        public Vector2 GetPointAtAngleDegrees(float angle)
        {
            angle = (float)(angle * (System.Math.PI / 180.0));
            return GetPointAtAngleRad(angle);
        }

        public Vector2 GetPointAtAngleRad(float angle)
        {
            float angleX = GetX() + (GetRadius() * Mathf.Cos(angle));
            float angleY = GetY() + (GetRadius() * Mathf.Sin(angle));

            return new Vector2(angleX, angleY);
        }



    }

}

