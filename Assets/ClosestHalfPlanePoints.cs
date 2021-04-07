using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClosestHalfPlanePoints
{
    private Dictionary<HalfPlane, Vector2?> closestPointOnPlaneDictionary = new Dictionary<HalfPlane, Vector2?>();

    private Vector2 pointToCalculateDistanceTo;
    private IPointPlaneAssignment pointPlaneLine;

    public ClosestHalfPlanePoints(Vector2 pointToCalculateDistanceTo, IPointPlaneAssignment pointPlaneLine)
    {
        InitializeClosestPointOnPlaneDictionary();
        SetPointToCalculateDistanceTo(pointToCalculateDistanceTo);
        SetPointPlaneLine(pointPlaneLine);
    }

    public (HalfPlane, Vector2?)[] GetAllPlanesAndClosestPoints()
    {
        HalfPlane[] allExistingPlanes = Enum.GetValues(typeof(HalfPlane)).Cast<HalfPlane>().ToArray();

        (HalfPlane, Vector2?)[] planesAndClosestPoints = new (HalfPlane, Vector2?)[allExistingPlanes.Length];

        for(int i = 0; i < allExistingPlanes.Length; ++i)
        {
            HalfPlane plane = allExistingPlanes[i];
            Vector2? closestPointOnPlane = GetClosestPointOnPlane(plane);

            planesAndClosestPoints[i] = (plane, closestPointOnPlane);

        }

        return planesAndClosestPoints;

    }

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

    private Dictionary<HalfPlane, Vector2?> GetClosestPointOnPlaneDictionary()
    {
        return closestPointOnPlaneDictionary;
    }

    private void SetClosestPointOnPlaneDictionary(Dictionary<HalfPlane, Vector2?> closestPointOnPlaneDictionary)
    {
        this.closestPointOnPlaneDictionary = closestPointOnPlaneDictionary;
    }

    private void InitializeClosestPointOnPlaneDictionary()
    {
        SetClosestPointOnPlaneDictionary(new Dictionary<HalfPlane, Vector2?>());
        Dictionary<HalfPlane, Vector2?> closestPointDictionary = GetClosestPointOnPlaneDictionary();

        HalfPlane[] planeValues = Enum.GetValues(typeof(HalfPlane)).Cast<HalfPlane>().ToArray();

        for(int i = 0; i < planeValues.Length; ++i)
        {
            HalfPlane planeValue = planeValues[i];
            closestPointDictionary.Add(planeValue, null);
        }
    }

    public Vector2? GetClosestPointOnPlane(HalfPlane plane)
    {
        return GetClosestPointOnPlaneDictionary()[plane];
    }

    private void SetClosestPointOnPlane(Vector2 point, HalfPlane plane)
    {
        GetClosestPointOnPlaneDictionary()[plane] = point;
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

        Vector2? closestPointOnPlane = GetClosestPointOnPlane(plane.Value);

        // If we don't yet have a closest point on that half plane, set
        // it to the parameter point.
        if(!closestPointOnPlane.HasValue)
        {
            SetClosestPointOnPlane(point, plane.Value);
            return;
        }

        float distanceToCurrentClosest = MathEBV.PointDistance(GetPointToCalculateDistanceTo(), closestPointOnPlane.Value);
        float distanceToParameter = MathEBV.PointDistance(GetPointToCalculateDistanceTo(), point);

        if(distanceToParameter < distanceToCurrentClosest)
        {
            SetClosestPointOnPlane(point, plane.Value);
        }

    }
    
}