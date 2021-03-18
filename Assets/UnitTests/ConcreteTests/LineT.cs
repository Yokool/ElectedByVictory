using UnityEngine;

public class LineT : MonoBehaviour, IUnitTestBundle
{
    public SingleUnitTest[] GetUnitTests()
    {
        string CC = nameof(LineT);
        IUnitTestNameProvider nameProvider = TestNameFactory.GetName_CC_TC_TM(CC, nameof(Line), nameof(Line.GetIntersectionWithLine));
        return new SingleUnitTest[1]
        {
            new SingleUnitTest(Test1, nameProvider)
        };
    }

    private bool LineIntersectionTest(Line l1, Line l2, Vector2? expectedIntersection)
    {
        Vector2? intersection = l1.GetIntersectionWithLine(l2);
        return MathEBV.PointsEqualNullable(intersection, expectedIntersection);
    }
    /// <summary>
    /// Standard test of two normal lines.
    /// Also assures that the order of l1 and l2 doesn't matter and they will produce the same result.
    /// </summary>
    /// <returns></returns>
    private bool Test1()
    {
        Line l1 = new Line(-0.5f, 0f, 0f, 5f);
        Line l2 = new Line(-1.2f, 0f, 0f, 6f);

        Vector2? expectedPoint = new Vector2(0.2f, 7f);

        return LineIntersectionTest(l1, l2, expectedPoint) && LineIntersectionTest(l1, l2, expectedPoint);
    }

}
