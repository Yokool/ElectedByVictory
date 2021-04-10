namespace ElectedByVictory.WorldCreation
{
    public static class MidpointLineUtilStatic
    {
        public static Line[] ExtractLines(MidpointLineUtil[] midpointLineUtils)
        {
            Line[] extractedLines = new Line[midpointLineUtils.Length];

            for(int i = 0; i < midpointLineUtils.Length; ++i)
            {
                MidpointLineUtil midpointLineUtil = midpointLineUtils[i];
                extractedLines[i] = midpointLineUtil.GetLine();
            }

            return extractedLines;
        }
    }
}

