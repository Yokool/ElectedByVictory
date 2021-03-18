using UnityEngine;

public class LineSegmentT : MonoBehaviour, IUnitTestBundle
{
    public SingleUnitTest[] GetUnitTests()
    {
        string CC = nameof(LineT);
        IUnitTestNameProvider nameProvider = TestNameFactory.GetName_CC_TC_TM(CC, nameof(LineSegment), nameof(LineSegment.GetPointAt));
        return new SingleUnitTest[3]
        {
            new SingleUnitTest(Test1, nameProvider),
            new SingleUnitTest(Test2, nameProvider),
            new SingleUnitTest(Test3, nameProvider)
        };
    }

    private bool LineTest(LineSegment line, float way, Vector2 expectedPosition)
    {
        return MathEBV.PointsEqual(line.GetPointAt(way), expectedPosition);
    }

    private bool Test1()
    {
        LineSegment l = new LineSegment(10f, 20f, 20f, 20f);
        return LineTest(l, 0.5f, new Vector2(15f, 20f));
    }

    private bool Test2()
    {
        LineSegment l = new LineSegment(10f, 20f, 20f, 20f);
        return LineTest(l, 0.25f, new Vector2(12.5f, 20f));
    }

    private bool Test3()
    {
        LineSegment l = new LineSegment(-10f, 20f, 20f, -40f);
        return LineTest(l, 0.5f, new Vector2(5f, -10f));
    }
}