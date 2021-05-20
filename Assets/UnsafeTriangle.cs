using ElectedByVictory.WorldCreation;
using UnityEngine;

public class UnsafeTriangle
{

    private Vector2 A;
    private Vector2 B;
    private Vector2 C;

    private Vector2 centroid;

    private Circle excircle;

    private void Update()
    {
        UpdateCentroid();
        UpdateExcircle();
    }

    private void UpdateCentroid()
    {
        Vector2 A = GetA();
        Vector2 B = GetB();
        Vector2 C = GetC();

        float centroidX = MathEBV.Average(A.x, B.x, C.x);
        float centroidY = MathEBV.Average(A.y, B.y, C.y);

        Vector2 centroid = new Vector2(centroidX, centroidY);

        SetCentroid(centroid);
    }

    private void UpdateExcircle()
    {
        Vector2 centroid = GetCentroid();
        Vector2 A = GetA();

        float radius = MathEBV.PointDistance(centroid, A);

        Circle excircle = new Circle(centroid, radius);
        SetExcircle(excircle);
    }

    private void SetCentroid(Vector2 centroid)
    {
        this.centroid = centroid;
    }

    private void SetExcircle(Circle excircle)
    {
        this.excircle = excircle;
    }

    public bool ExcircleContainsPoint(Vector2 point)
    {
        return GetExcircle().ContainsPoint(point);
    }

    public Vector2 GetCentroid()
    {
        return this.centroid;
    }

    public Circle GetExcircle()
    {
        return this.excircle;
    }

    public override string ToString()
    {
        Vector2 A = GetA();
        Vector2 B = GetB();
        Vector2 C = GetC();

        return $"{nameof(UnsafeTriangle)} A: {{{A}}} B: {{{B}}} C: {{{C}}}";

    }

    /// <summary>
    /// Unsafe as it can create triangles out of invalid points.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    public UnsafeTriangle(Vector2 A, Vector2 B, Vector2 C)
    {
        this.SetA(A);
        this.SetB(B);
        this.SetC(C);

        this.Update();
    }



    private void SetA(Vector2 A)
    {
        this.A = A;
    }

    private void SetB(Vector2 B)
    {
        this.B = B;
    }

    private void SetC(Vector2 C)
    {
        this.C = C;
    }

    public Vector2 GetA()
    {
        return this.A;
    }

    public Vector2 GetB()
    {
        return this.B;
    }

    public Vector2 GetC()
    {
        return this.C;
    }
}
