using UnityEngine;

public static class LineArrayUtilities
{
    /*
    public static Line[] GetShallowPerpendicularLinesFromArray(Line[] lines)
    {
        Line[] shallowPerpendicularLines = new Line[lines.Length];
        for(int i = 0; i < lines.Length; ++i)
        {
            Line line = lines[i];
            shallowPerpendicularLines[i] = line.GetShallowPerpendicularLine();
        }
        return shallowPerpendicularLines;
    }

    public static Line[] GetDeepPerpendicularLinesFromArray(Line[] lines, Vector2 travelThrough)
    {
        // Shallow at this point
        Line[] deepLines = GetShallowPerpendicularLinesFromArray(lines);

        // Make them deep
        for(int i = 0; i < deepLines.Length; ++i)
        {
            Line deepLine = deepLines[i];
            deepLine.SetTravelThroughPoint(travelThrough);
        }
        return deepLines;
    }
    */
}
