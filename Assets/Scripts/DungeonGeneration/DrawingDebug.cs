using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawingDebug
{
    public static void DrawCrossBox(Vector3 min, Vector3 max, Color colour, float time = 100) {
        Debug.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0), colour, time);
        Debug.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(min.x, max.y, 0), colour, time);
        Debug.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, max.y, 0), colour, time);
        Debug.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0), colour, time);
        Debug.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(max.x, min.y, 0), colour, time);
        Debug.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(max.x, min.y, 0), colour, time);
    }

    public static void DrawCrossBox(Vector3 point, Color colour, float time = 100) {
        DrawCrossBox(new Vector3(point.x + 0.5f, point.y + 0.5f, 0), new Vector3(point.x - 0.5f, point.y - 0.5f, 0), colour, time);
    }
}
