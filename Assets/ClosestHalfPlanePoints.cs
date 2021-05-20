using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClosestHalfPlanePoints
{
    private Dictionary<HalfPlane, List<Vector2?>> closestPointOnPlaneDictionary = null;

    private Vector2 pointToCalculateDistanceTo;
    private IPointPlaneAssignment pointPlaneLine;
    private int numberOfPointsPerPlane;

    public ClosestHalfPlanePoints(Vector2 pointToCalculateDistanceTo, IPointPlaneAssignment pointPlaneLine, int numberOfPointsPerPlane)
    {
        SetNumberOfPointsPerPlane(numberOfPointsPerPlane);
        InitializeClosestPointOnPlaneDictionary();
        SetPointToCalculateDistanceTo(pointToCalculateDistanceTo);
        SetPointPlaneLine(pointPlaneLine);
    }

    /// <summary>
    /// Returns the number of non-null points for the plane with the smallest number of them.
    /// 
    /// IE:
    /// HalfPlaneA - 10
    /// HalfPlaneB - 7
    /// HalfPlaneC - 9
    /// 
    /// Return 7
    /// 
    /// </summary>
    public int MinNonNullPointCountForAllPlanes()
    {
        HalfPlane[] planes = HalfPlaneUtilities.GetAllPlanes();

        // This is the largest that the value can ever be.
        int minNonNullCount = GetNumberOfPointsPerPlane();

        for (int i = 0; i < planes.Length; ++i)
        {
            HalfPlane plane = planes[i];

            int nonNullPointsOnPlane = NonNullPointsOnPlaneCount(plane);

            if (nonNullPointsOnPlane < minNonNullCount)
            {
                minNonNullCount = nonNullPointsOnPlane;
            }

        }

        return minNonNullCount;
    }

    public bool NoPlaneHasMoreThanParameterPoints(int number)
    {
        return (MaxNonNullPointsForAllPlanes() == number);
    }

    public int MaxNonNullPointsForAllPlanes()
    {
        HalfPlane[] planes = HalfPlaneUtilities.GetAllPlanes();

        int maxNonNullCount = 0;

        for (int i = 0; i < planes.Length; ++i)
        {
            HalfPlane plane = planes[i];

            int nonNullPointsOnPlane = NonNullPointsOnPlaneCount(plane);

            if (nonNullPointsOnPlane > maxNonNullCount)
            {
                maxNonNullCount = nonNullPointsOnPlane;
            }

        }

        return maxNonNullCount;
    }

    public bool HasEveryPlaneAtLeastOnePoint()
    {
        return AtLeastXNonNullPointsOnEachPlane(1);
    }

    public bool AtLeastXNonNullPointsOnEachPlane(int count)
    {
        HalfPlane[] planes = HalfPlaneUtilities.GetAllPlanes();

        for(int i = 0; i < planes.Length; ++i)
        {
            HalfPlane plane = planes[i];

            int nonNullPointsOnPlane = NonNullPointsOnPlaneCount(plane);

            if(nonNullPointsOnPlane < count)
            {
                return false;
            }

        }

        return true;
    }

    public int NonNullPointsOnAllPlanesCount()
    {
        HalfPlane[] planes = HalfPlaneUtilities.GetAllPlanes();
        int nonNullCount = 0;

        for(int i = 0; i < planes.Length; ++i)
        {
            HalfPlane plane = planes[i];
            nonNullCount += NonNullPointsOnPlaneCount(plane);
        }
        return nonNullCount;
    }

    public int NullPointsOnAllPlanesCount()
    {
        int nonNullCount = NonNullPointsOnAllPlanesCount();

        HalfPlane[] planes = HalfPlaneUtilities.GetAllPlanes();
        int planeCount = planes.Length;

        int nullCount = (GetNumberOfPointsPerPlane() * planeCount) - nonNullCount;

        return nullCount;
    }

    public int NonNullPointsOnPlaneCount(HalfPlane plane)
    {
        List<Vector2?> closePointsOnPlane = GetClosePointsOnPlane(plane);

        int nonNullCount = 0;

        for(int i = 0; i < closePointsOnPlane.Count; ++i)
        {
            Vector2? closestPoint = closePointsOnPlane[i];

            if(closestPoint.HasValue)
            {
                ++nonNullCount;
            }

        }

        return nonNullCount;
    }

    public int NullPointsOnPlaneCount(HalfPlane plane)
    {
        return (GetNumberOfPointsPerPlane() - NonNullPointsOnPlaneCount(plane));
    }

    public Vector2?[] GetClosestPointsOnAPlaneCopy(HalfPlane plane)
    {
        List<Vector2?> closestPointsForAPlane = GetClosePointsOnPlane(plane);
        Vector2?[] copy = new Vector2?[closestPointsForAPlane.Count];

        for(int i = 0; i < closestPointsForAPlane.Count; ++i)
        {
            copy[i] = closestPointsForAPlane[i];
        }

        return copy;
    }

    public Vector2? GetFurthestPointForPlane(HalfPlane plane)
    {
        List<Vector2?> closePointsOnPlane = GetClosePointsOnPlane(plane);

        for(int i = closePointsOnPlane.Count; i > 0; --i)
        {
            Vector2? closePointOnPlane = closePointsOnPlane[i];

            if(closePointOnPlane.HasValue)
            {
                return closePointOnPlane;
            }
        }

        throw new Exception("No furthest point for plane found.");
    }

    public Vector2? GetClosestPointForPlane(HalfPlane plane)
    {
        return GetClosestPointOnPlaneByIndex(plane, 0);
    }

    /*
    private (HalfPlane, List<Vector2?>)[] GetAllPlanesAndClosePoints()
    {
        HalfPlane[] allExistingPlanes = Enum.GetValues(typeof(HalfPlane)).Cast<HalfPlane>().ToArray();

        (HalfPlane, List<Vector2?>)[] planesAndClosePoints = new (HalfPlane, List<Vector2?>)[allExistingPlanes.Length];

        for(int i = 0; i < allExistingPlanes.Length; ++i)
        {
            HalfPlane plane = allExistingPlanes[i];
            List<Vector2?> closePointsOnPlane = GetClosestPointsOnPlane(plane);

            planesAndClosePoints[i] = (plane, closePointsOnPlane);

        }

        return planesAndClosePoints;
    }
    */

    public void SetPointToCalculateDistanceTo(Vector2 pointToCalculateDistanceTo)
    {
        this.pointToCalculateDistanceTo = pointToCalculateDistanceTo;
    }

    public Vector2 GetPointToCalculateDistanceTo()
    {
        return this.pointToCalculateDistanceTo;
    }

    public IPointPlaneAssignment GetPointPlaneLine()
    {
        return this.pointPlaneLine;
    }

    public void SetPointPlaneLine(IPointPlaneAssignment pointPlaneLine)
    {
        this.pointPlaneLine = pointPlaneLine;
    }

    private Dictionary<HalfPlane, List<Vector2?>> GetClosestPointOnPlaneDictionary()
    {
        return closestPointOnPlaneDictionary;
    }

    private void SetClosestPointOnPlaneDictionary(Dictionary<HalfPlane, List<Vector2?>> closestPointOnPlaneDictionary)
    {
        this.closestPointOnPlaneDictionary = closestPointOnPlaneDictionary;
    }

    private void InitializeClosestPointOnPlaneDictionary()
    {
        SetClosestPointOnPlaneDictionary(new Dictionary<HalfPlane, List<Vector2?>>());
        Dictionary<HalfPlane, List<Vector2?>> closestPointDictionary = GetClosestPointOnPlaneDictionary();

        HalfPlane[] planeValues = Enum.GetValues(typeof(HalfPlane)).Cast<HalfPlane>().ToArray();

        for(int i = 0; i < planeValues.Length; ++i)
        {
            HalfPlane planeValue = planeValues[i];
            InitializeClosestPointsOnHalfPlane(planeValue);
        }
    }

    private void InitializeClosestPointsOnHalfPlane(HalfPlane plane)
    {
        GetClosestPointOnPlaneDictionary().Add(plane, new List<Vector2?>());
        List<Vector2?> closestPointsOnPlane = GetClosePointsOnPlane(plane);

        for(int i = 0; i < numberOfPointsPerPlane; ++i)
        {
            closestPointsOnPlane.Add(null);
        }
    }

    private List<Vector2?> GetClosePointsOnPlane(HalfPlane plane)
    {
        return GetClosestPointOnPlaneDictionary()[plane];
    }

    private void SetClosestPointOnPlaneByIndex(HalfPlane plane, int index, Vector2? point)
    {
        GetClosePointsOnPlane(plane)[index] = point;
    }

    /// <summary>
    /// Sorts the points on the halfplane. If we have <see cref="numberOfPointsPerPlane"/> = 1, then this
    /// method doesn't do anything.
    /// </summary>
    /// <param name="plane"></param>
    private void ReorganizeClosestPointsHalfPlane(HalfPlane plane)
    {
        for(int i = 0; i < numberOfPointsPerPlane; ++i)
        {
            for(int j = (i + 1); j < numberOfPointsPerPlane; ++j)
            {
                Vector2? current = GetClosestPointOnPlaneByIndex(plane, i);
                Vector2? other = GetClosestPointOnPlaneByIndex(plane, j);

                // If we arrive at the first null value, don't continue further as there are
                // no non-null values beyond the first one.
                // ---
                // I'm pretty sure we don't have to check for current.HasValue unless
                // we have a malformed array.
                if(!other.HasValue)
                {
                    break;
                }

                float currentDistance = GetDistanceToDistancePoint(current.Value);
                float otherDistance = GetDistanceToDistancePoint(other.Value);

                if (otherDistance < currentDistance)
                {
                    SetClosestPointOnPlaneByIndex(plane, i, other);
                    SetClosestPointOnPlaneByIndex(plane, j, current);
                }
            }
        }
    }


    private Vector2? GetClosestPointOnPlaneByIndex(HalfPlane plane, int index)
    {
        return GetClosePointsOnPlane(plane)[index];
    }

    private float GetDistanceToDistancePoint(Vector2 point)
    {
        return MathEBV.PointDistance(GetPointToCalculateDistanceTo(), point);
    }

    public void TrySetPointAsClosest(Vector2 point)
    {
        HalfPlane? plane = pointPlaneLine.GetLinePlaneAssignment(point);

        // Ignore points that are directly on the line, we can't assign
        // them to a plane as there is ambiguity.
        if(plane.HasValue)
        {
            return;
        }

        List<Vector2?> closePointsOnPlane = GetClosePointsOnPlane(plane.Value);

        for(int i = 0; i < numberOfPointsPerPlane; ++i)
        {
            Vector2? closePointOnPlane = closePointsOnPlane[i];

            // If we have a free space in our closestPointsArray - we can assign it directly
            if(!closePointOnPlane.HasValue)
            {
                SetClosestPointOnPlaneByIndex(plane.Value, i, point);

                // Since we're first checking for a free space and assigning it to the parameter
                // point if we find it, it might happen that the inserted point is closer than an already present
                // point - therefore we must resort the plane array.
                ReorganizeClosestPointsHalfPlane(plane.Value);
                return;
            }

        }

        TrySetPointAsClosestAssertedToPlane(plane.Value, point);

    }

    /// <summary>
    /// If we have asserted that the parameter point is on the parameter half plane, we can
    /// try to replace one of the half plane points if the point is closer than any point already present in the array.
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="point"></param>
    private void TrySetPointAsClosestAssertedToPlane(HalfPlane plane, Vector2 point)
    {
        float parameterDistance = GetDistanceToDistancePoint(point);

        for (int i = 0; i < numberOfPointsPerPlane; ++i)
        {
            Vector2? closePoint = GetClosestPointOnPlaneByIndex(plane, i);
            float closePointDistance = GetDistanceToDistancePoint(closePoint.Value);

            if (parameterDistance < closePointDistance)
            {
                SetClosestPointOnPlaneByIndex(plane, i, point);

                // Think of the distances by the parameter:
                // First call: 0.4 => {0.6, 0.8}  [0.4 replaces 0.6]
                // Recursive call: 0.6 => {0.4, 0.8} [0.6 who was formerly in the list now replaces 0.8 as it is closer]
                // Recursive call 2: 0.8 => {0.4, 0.6} [Recursion ends as all the points in the list are closer than the parameter point and no
                //                                      point is knocked out]
                // Result: {0.4, 0.6}

                // All the points here will have a value, as we can't get here if there is a null point
                // in the close points list.
                // Nevertheless if we replace the close point in the list by the one in the parameter,
                // we must see if the replaced point can't also replace a difference point in the array.
                TrySetPointAsClosestAssertedToPlane(plane, closePoint.Value);
            }
        }
    }

    private void SetNumberOfPointsPerPlane(int numberOfPointsPerPlane)
    {
        this.numberOfPointsPerPlane = numberOfPointsPerPlane;
    }

    public int GetNumberOfPointsPerPlane()
    {
        return this.numberOfPointsPerPlane;
    }
    
}