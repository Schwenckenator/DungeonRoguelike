using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Voxel2D
{
    public bool state;

    public Vector2 position, xEdgePosition, yEdgePosition;

    public Voxel2D(int x, int y) {
        position.x = x;
        position.y = y;

        xEdgePosition = position;
        xEdgePosition.x += 0.5f;

        yEdgePosition = position;
        yEdgePosition.y += 0.5f;
    }
}
