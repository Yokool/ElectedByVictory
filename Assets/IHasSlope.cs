public interface IHasSlope
{
    float? GetSlope();
    //void SetSlope(float? m);
    //void SetSlope(float x1, float y1, float x2, float y2);

    float? GetPerpendicularSlope();

    bool IsVertical();
    bool IsHorizontal();
}
