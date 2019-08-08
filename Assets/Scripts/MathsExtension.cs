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

    public static float RoundToValue(this float input, float fraction) {
        float output = input;
        output -= fraction;
        output = Mathf.Round(output);
        output += fraction;

        return output;
    }
       
}