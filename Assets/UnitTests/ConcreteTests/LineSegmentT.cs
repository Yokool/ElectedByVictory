using UnityEngine;

public class LineSegmentT : MonoBehaviour, IUnitTestBundle
{
    public SingleUnitTest[] GetUnitTests()
    {
        string CC = nameof(LineT);
        IUnitTestNameProvider nameProvider = TestNameFactory.GetName_CC_TC_TM(CC, nameof(LineSegment), nameof(LineSegment.GetPointAt));
        return new SingleUnitTest[1]
        {
            /*
            new SingleUnitTest(AtWayTest05, nameProvider),
            new SingleUnitTest(AtWayTest025, nameProvider),
            new SingleUnitTest(AtWayTest05_2, nameProvider),
            new SingleUnitTest(IntersectionEndpointTest, nameProvider),
            new SingleUnitTest(NoIntersectionTest, nameProvider),
            new SingleUnitTest(NormalIntersectionTest, nameProvider),
            new SingleUnitTest(HorizontalVerticalIntersectionTest, nameProvider),
            */
            new SingleUnitTest(PolygonSegmentBoundariesIntersectionTest, nameProvider)
        };
    }

    private bool LineTest(LineSegment line, float way, Vector2 expectedPosition)
    {
        return MathEBV.PointsEqual(line.GetPointAt(way), expectedPosition);
    }

    private bool AtWayTest05()
    {
        LineSegment l = new LineSegment(10f, 20f, 20f, 20f);
        return LineTest(l, 0.5f, new Vector2(15f, 20f));
    }

    private bool AtWayTest025()
    {
        LineSegment l = new LineSegment(10f, 20f, 20f, 20f);
        return LineTest(l, 0.25f, new Vector2(12.5f, 20f));
    }

    private bool AtWayTest05_2()
    {
        LineSegment l = new LineSegment(-10f, 20f, 20f, -40f);
        return LineTest(l, 0.5f, new Vector2(5f, -10f));
    }

    private bool IntersectionEndpointTest()
    {
        Vector2 endpoint1 = new Vector2(1, 1);
        Vector2 sharedEndpoint = new Vector2(2, 1);
        Vector2 endpoint2 = new Vector2(2, 2);

        return LineSegmentIntersectionTest(endpoint1, sharedEndpoint, sharedEndpoint, endpoint2, sharedEndpoint);
    }

    private bool NoIntersectionTest()
    {
        return LineSegmentIntersectionTest(new Vector2(-3.5f, 2.5f), new Vector2(-3f, 2f), new Vector2(3f, 2f), new Vector2(3.5f, 2.5f), null);
    }

    private bool NormalIntersectionTest()
    {
        return LineSegmentIntersectionTest(new Vector2(-0.4f, -0.8f), new Vector2(0.4f, -1f), new Vector2(-0.4f, -1f), new Vector2(0.4f, -0.8f), new Vector2(0f, -0.9f));
    }

    private bool HorizontalVerticalIntersectionTest()
    {
        return LineSegmentIntersectionTest(new Vector2(0.6f, -0.7f), new Vector2(0.6f, -0.6f), new Vector2(0.55f, -0.65f), new Vector2(0.65f, -0.65f), new Vector2(0.6f, -0.65f));
    }

    private bool LineSegmentIntersectionTest(Vector2 endpoint1, Vector2 endpoint2, Vector2 endpoint3, Vector2 endpoint4, Vector2? expectedIntersection)
    {
        return LineIntersectionTest(new LineSegment(endpoint1, endpoint2), new LineSegment(endpoint3, endpoint4), expectedIntersection);
    }

    private bool LineIntersectionTest(LineSegment l1, LineSegment l2, Vector2? expectedIntersection)
    {
        Vector2? intersection = l1.GetIntersectionWithLineSegment(l2);
        return MathEBV.PointsEqualNullable(intersection, expectedIntersection);
    }

    private bool PolygonSegmentBoundariesIntersectionTest()
    {
        /*
        Vector2 A = new Vector2(189.5f, -177.9f);
        Vector2 B = new Vector2(220.8f, -199.2f);
        LineSegment AB = new LineSegment(A, B);

        Vector2 C = new Vector2(96.4f, -254.2f);
        Vector2 D = new Vector2(220.8f, -199.2f);
        LineSegment CD = new LineSegment(C, D);
        
        Vector2 E = new Vector2(99f, -156.4f);
        Vector2 F = new Vector2(96.4f, -254.2f);
        LineSegment EF = new LineSegment(E, F);
        
        Vector2 G = new Vector2(99f, -156.4f);
        Vector2 H = new Vector2(189.5f, -177.9f);
        LineSegment GH = new LineSegment(G, H);

        ILineRaySegmentUnion[] lines = new ILineRaySegmentUnion[4]{AB, CD, EF, GH};

        Debug.Log("A: " + GH.GetIntersectionWithLineSegment(AB));
        */

        Vector2 A = new Vector2(-225.6f, 458.1f);
        Vector2 B = new Vector2(-248.7f, 415.4f);
        LineSegment AB = new LineSegment(A, B);

        Vector2 C = new Vector2(-248.9f, 494.8f);
        Vector2 D = new Vector2(-248.7f, 415.4f);
        LineSegment CD = new LineSegment(C, D);

        Vector2 E = new Vector2(-248.9f, 494.8f);
        Vector2 F = new Vector2(-225.6f, 458.1f);
        LineSegment EF = new LineSegment(E, F);

        ILineRaySegmentUnion[] lines = new ILineRaySegmentUnion[3] { AB, CD, EF };


        Debug.Log(EF.GetIntersectionWithLineSegment(CD));

        //Vector2[] intersections = LineUtilities.GetAllUniqueIntersections(lines);

        /*
        for(int i = 0; i < intersections.Length; ++i)
        {
            Debug.Log(intersections[i]);
        }

        Debug.Log(intersections.Length);
        */
        return true;
    }

}