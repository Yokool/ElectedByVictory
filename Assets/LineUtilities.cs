using UnityEngine;

public static class LineUtilities
{
    /// <summary>
    /// Tries to get the intersection point for two lines, when the following condition is met:
    /// L1 = vertical || L2 = horizontal
    /// L2 = horizontal || L1 = vertical
    /// 
    /// As long as m1 = null or m2 = null, this method will handle the scenario.
    /// 
    /// </summary>
    /// <returns>True if this method handled the line intersection, it follows then that the user can use intersectionPoint out variable.</returns>
    public static bool TrySpecialLineIntersection(float? m1, float b1, float? m2, float b2, out Vector2? intersectionPoint)
    {
        intersectionPoint = null;

        bool L1Vertical = !m1.HasValue;
        bool L2Vertical = !m2.HasValue;

        // If both lines are not vertical, then we don't process it inside of this method
        if(!L1Vertical && !L2Vertical)
        {
            return false;
        }

        // Both lines are vertical, thus they are parallel
        // For lines if their x location is different, then they will never
        // meet.
        // They could also overlap each other but with lines we can't get a sensible
        // point.
        if(L1Vertical && L2Vertical)
        {
            return true;
        }

        // L1 = vertical => L2 = horizontal
        if (L1Vertical)
        {
            intersectionPoint = LeftNormalRightVertical(m2.Value, b2, b1);
        }
        // L2 = vertical => L1 = horizontal
        else if (L2Vertical)
        {
            intersectionPoint = LeftNormalRightVertical(m1.Value, b1, b2);
        }

        return true;

    }

    private static Vector2 LeftNormalRightVertical(float m1, float b1, float b2)
    {
        float x = b2;
        float y = m1 * b2 + b1;
        return new Vector2(x, y);
    }


    public static float GetYInterceptFromPoint(float? m, float x, float y)
    {
        //x = b
        if (!m.HasValue)
        {
            return x;
        }

        float b = y - m.Value * x;
        return b;
    }

    


}
