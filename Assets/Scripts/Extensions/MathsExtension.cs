using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsExtension {
    #region Vector2
    public static Vector2Int RoundToInt(this Vector2 input) {
        return new Vector2Int(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
    }
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

    public static Vector3 ToVector3(this Vector2 input) {
        return new Vector3(input.x, input.y, 0);
    }
    #endregion

    #region Vector2Int
    public static Vector2Int Rotate90(this Vector2Int input, int times = 1) {


        Vector2Int output = input;

        for (int i = 0; i < times; i++) {

            output = new Vector2Int(-output.y, output.x);
        }
        //Debug.Log($"Rotate 90! input was {input.ToString()}, output is {output.ToString()}!");
        return output;
    }

    /// <summary>
    /// Returns a Vector3Int, with a zero z value.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>Returns a Vector3Int, with a zero z value.</returns>
    public static Vector3Int ToVector3Int(this Vector2Int input) {
        return new Vector3Int(input.x, input.y, 0);
    }
    public static Vector2Int DivideByScalar(this Vector2Int input, int scalar) {
        return new Vector2Int(input.x / scalar, input.y / scalar);
    }
    #endregion

    #region Vector3
    public static Vector2[] ToVector2s(this Vector3[] input) {
        var vector2s = new List<Vector2>();
        foreach(Vector3 v3 in input) {
            vector2s.Add(new Vector2(v3.x, v3.y));
        }
        return vector2s.ToArray();
    }

    public static Vector3Int RoundToInt(this Vector3 input) {
        return new Vector3Int(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y), Mathf.RoundToInt(input.z));
    }
    public static Vector3Int FloorToInt(this Vector3 input) {
        return new Vector3Int(Mathf.FloorToInt(input.x), Mathf.FloorToInt(input.y), Mathf.FloorToInt(input.z));
    }
    public static Vector3Int CeilToInt(this Vector3 input) {
        return new Vector3Int(Mathf.CeilToInt(input.x), Mathf.CeilToInt(input.y), Mathf.CeilToInt(input.z));
    }
    #endregion

    #region Vector3Int
    /// <summary>
    /// Returns a Vector2Int, throwing away the z parameter.
    /// </summary>
    /// <returns>Returns a Vector2Int, throwing away the z parameter.</returns>
    public static Vector2Int ToVector2Int(this Vector3Int input) {
        return new Vector2Int(input.x, input.y);
    }
    #endregion

    public static float RoundToValue(this float input, float fraction) {
        float output = input;
        output -= fraction;
        output = Mathf.Round(output);
        output += fraction;

        return output;
    }

    public static bool IsWithin(this float value, float minimum, float maximum)
    {
        return value >= minimum && value <= maximum;
    }
}