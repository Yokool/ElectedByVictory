using UnityEngine;

namespace ElectedByVictory.WorldCreation
{
    public class MidpointLineUtil
    {
        private Vector2 startPoint;
        private Vector2 endPoint;


        private Line cachedMidpointLine;
        private Vector2 cachedMidpoint;

        public MidpointLineUtil(Vector2 startPoint, Vector2 endPoint)
        {
            SetStartAndEndPointRecalc(startPoint, endPoint);
        }

        public void SetStartAndEndPointRecalc(Vector2 startPoint, Vector2 endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            Recalculate();
        }

        public Vector2 GetStartPoint()
        {
            return this.startPoint;
        }

        public Vector2 GetEndPoint()
        {
            return this.endPoint;
        }

        public void Recalculate()
        {
            RecalculateMidpoint();
            RecalculateLine();
        }

        private void RecalculateLine()
        {
            LineSegment startToEndLine = GetLineSegmentFromStartToEnd();
            Line midpointLine = startToEndLine.GetMidpointLine();
            _SetCachedLine(midpointLine);
        }

        private LineSegment GetLineSegmentFromStartToEnd()
        {
            Vector2 startPoint = GetStartPoint();
            Vector2 endPoint = GetEndPoint();
            return new LineSegment(startPoint, endPoint);
        }

        private void RecalculateMidpoint()
        {
            LineSegment lineFromStartToEnd = GetLineSegmentFromStartToEnd();
            Vector2 midpoint = lineFromStartToEnd.GetPointAt(0.5f);

            _SetCachedMidpoint(midpoint);
        }

        private void _SetCachedLine(Line cachedMidpointLine)
        {
            this.cachedMidpointLine = cachedMidpointLine;
        }

        private void _SetCachedMidpoint(Vector2 cachedMidpoitn)
        {
            this.cachedMidpoint = cachedMidpoitn;
        }

        public Line GetLine()
        {
            return this.cachedMidpointLine;
        }

        public Vector2 GetMidpoint()
        {
            return this.cachedMidpoint;
        }

    }
}

