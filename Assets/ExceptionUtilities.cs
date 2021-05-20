using System;
using UnityEngine;

public static class ExceptionUtilities
{
    public static Exception CODE_BLOCK_SHOULD_NOT_BE_REACHED_EXCEPTION()
    {
        Debug.Log("THIS_CODE_BLOCK_SHOULD_NOT_BE_REACHED");
        return new Exception("ILLEGAL_CODE_BLOCK_REACHED");
    }

    public static Exception ILLEGAL_LINE_EXCEPTION(Line line, float x1, float y1, float x2, float y2)
    {
        Debug.LogError($"Tried to set a slope of the line: {line} - out of two points a, b where a = b; a = {{x: {x1} y: {y1}}}; b = {{x: {x2} y. {y2}}}. This is not possible, we need two non-equal points to construct a line.");
        return new Exception("ILLEGAL_LINE_EXCEPTION");
    }
}