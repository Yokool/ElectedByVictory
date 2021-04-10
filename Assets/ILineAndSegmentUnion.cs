public interface ILineAndSegmentUnion : IHasSlopeAndYIntercept, ILineLegality, IGetLineValuesAtPoint, ICanIntersectLineAndSegmentUnion, IPointPlaneAssignment
{
    Line GetShallowPerpendicularLine();
}