using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PointUtilitiesT : MonoBehaviour, IUnitTestBundle
{
    public SingleUnitTest[] GetUnitTests()
    {
        string CC = nameof(PointUtilitiesT);
        IUnitTestNameProvider info = TestNameFactory.GetName_CC_TC_TM(CC, nameof(PointUtilities), nameof(PointUtilities.ClampPointToLine));

        return new SingleUnitTest[4]
        {
            new SingleUnitTest(Test1, info),
            new SingleUnitTest(Test2, info),
            new SingleUnitTest(Test3, info),
            new SingleUnitTest(Test4, info)
        };
    }

    private static bool ClampPointToLineTest(float p1_x, float p1_y, float p2_x, float p2_y, float exp_x, float exp_y)
    {
        return ClampPointToLineTest(new Vector2(p1_x, p1_y), new Vector2(p2_x, p2_y), new Vector2(exp_x, exp_y));
    }

    private static bool ClampPointToLineTest(Vector2 p1, Vector2 p2, Vector2 expectedValue)
    {
        LineSegment ls = new LineSegment(p1.x, p1.y, p2.x, p2.y);

        Line clampingLine = ls.GetLinePerpendicularAtWay(0.5f);

        p2 = PointUtilities.ClampPointToLine(p1, p2, clampingLine);
        
        Debug.Log(p2);

        return MathEBV.PointsEqual(p2, expectedValue);
    }

    private bool Test1()
    {
        return ClampPointToLineTest(20f, 20f, 40f, 20f, 30f, 20f);
    }

    private bool Test2()
    {
        return ClampPointToLineTest(10f, 50f, 20f, 70f, 15f, 60f);
    }

    private bool Test3()
    {
        return ClampPointToLineTest(-20f, -10f, -90f, -20f, -55f, -15f);
    }

    private bool Test4()
    {
        return ClampPointToLineTest(10f, 30f, 10f, 10f, 10f, 20f);
    }


}
