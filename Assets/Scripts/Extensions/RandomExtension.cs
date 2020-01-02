using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtension
{
    public static T RandomItem<T>(this List<T> input) {
        int index = Random.Range(0, input.Count);
        return input[index];
    }

    public static T RandomItem<T>(this T[] input) {
        int index = Random.Range(0, input.Length);
        return input[index];
    }

    public static Vector2Int CardinalDirection() {
        int index = Random.Range(0, 4);
        switch (index) {
            case 0:
                return Vector2Int.right;
            case 1:
                return Vector2Int.up;
            case 2:
                return Vector2Int.left;
            case 3:
                return Vector2Int.down;
            default:
                throw new System.Exception("How in the fuck did it pick a number higher than 3???");
        }
    }
}
