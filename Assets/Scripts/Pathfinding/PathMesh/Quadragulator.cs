using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadragulator
{
    private readonly List<Vector2> points = new List<Vector2>();

    public Quadragulator(Vector2[] points) {
        this.points = new List<Vector2>(points);
    }

    public int[] TriangulateQuads() {
        List<int> indices = new List<int>();

        int n = points.Count;
        if (n < 4) { // Expecting at least 1 quad
            return indices.ToArray();
        }

        int[] vertices = new int[n];

        throw new System.NotImplementedException("Not done stooge.");
    }
}
