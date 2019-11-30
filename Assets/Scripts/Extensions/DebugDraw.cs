using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugDraw
{
    public static void CrossBox(Vector3 min, Vector3 max, Color colour, float time = 100) {
        Debug.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0), colour, time);
        Debug.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(min.x, max.y, 0), colour, time);
        Debug.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, max.y, 0), colour, time);
        Debug.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0), colour, time);
        Debug.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(max.x, min.y, 0), colour, time);
        Debug.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(max.x, min.y, 0), colour, time);
    }

    public static void CrossBox(Vector3 point, Color colour, float time = 100) {
        CrossBox(new Vector3(point.x + 0.5f, point.y + 0.5f, 0), new Vector3(point.x - 0.5f, point.y - 0.5f, 0), colour, time);
    }

    public static void Line(Vector3 start, Vector3 end, Color colour, float time = 100) {
        Debug.DrawLine(start, end, colour, time);
    }
}
