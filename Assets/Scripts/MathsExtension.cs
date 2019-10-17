using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsExtension {

    public static Vector2 RotateDeg(this Vector2 input, float degrees) {
        Vector2 output = Vector2.zero;

        float theta = Mathf.Deg2Rad * degrees;

        float cos = Mathf.Cos(theta);
        float sin = Mathf.Sin(theta);

        float xprime = input.x * cos - input.y * sin;
        float yprime = input.x * sin + input.y * cos;

        output = new Vector2(xprime, yprime);

        Debug.Log($"Rotation! Input:{input.ToString()} is now {output.ToString()}");

        return output;
    }

    public static Vector2Int RoundToInt(this Vector2 input) {
        return new Vector2Int(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
    }

    /// <summary>
    /// Returns a Vector2Int, throwing away the z parameter.
    /// </summary>
    /// <returns>Returns a Vector2Int, throwing away the z parameter.</returns>
    public static Vector2Int ToVector2Int(this Vector3Int input) {
        return new Vector2Int(input.x, input.y);
    }

    /// <summary>
    /// Returns a Vector3Int, with a zero z value.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>Returns a Vector3Int, with a zero z value.</returns>
    public static Vector3Int ToVector3Int(this Vector2Int input) {
        return new Vector3Int(input.x, input.y, 0);
    }

    public static float RoundToValue(this float input, float fraction) {
        float output = input;
        output -= fraction;
        output = Mathf.Round(output);
        output += fraction;

        return output;
    }
       
}