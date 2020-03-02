using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension 
{

    public static T[] Populate<T> (this T[] input, T value) {
        T[] output = new T[input.Length];
        for (int i=0; i < input.Length; i++) {
            output[i] = value;
        }
        return output;
    }

    public static T[,] Populate<T> (this T[,] input, T value) {
        T[,] output = new T[input.GetUpperBound(0), input.GetUpperBound(1)];
        for (int x = 0; x < input.GetUpperBound(0); x++) {
            for (int y = 0; y < input.GetUpperBound(1); y++) {
                output[x,y] = value;
            }
        }
        return output;
    }
}
