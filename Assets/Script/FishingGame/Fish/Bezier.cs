using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector2 GetPoint(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * control + t * t * end;
    }
    
    public static Vector2 GetDerivative(Vector2 start, Vector2 control, Vector2 end, float t)
    {
        t = Mathf.Clamp01(t);
        return 2f * ((1f - t) * (control - start) + t * (end - control));
    }
}

