using System;
using System.Linq;

public static class HalfPlaneUtilities
{
    public static HalfPlane[] GetAllPlanes()
    {
        return Enum.GetValues(typeof(HalfPlane)).Cast<HalfPlane>().ToArray();
    }
}