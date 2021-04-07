public interface IHasSlopeAndYIntercept : IHasSlope, IHasYIntercept
{
    void SetLineValues(float? m, float b);
    void SetLineValuesFromPoints(float x1, float y1, float x2, float y2);
}
