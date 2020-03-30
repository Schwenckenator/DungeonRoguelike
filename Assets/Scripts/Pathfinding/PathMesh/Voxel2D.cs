using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Voxel2D
{
    public bool state;

    public Vector2 position, xEdgePosition, yEdgePosition;
    public Vector2 topLeftPos, topRightPos, botRightPos, botLeftPos;

    public Voxel2D(int x, int y) {
        position.x = x;// - 0.5f;
        position.y = y;// - 0.5f;

        xEdgePosition = position;
        xEdgePosition.x += 0.5f;

        yEdgePosition = position;
        yEdgePosition.y += 0.5f;

        float offset = 0.5f;

        topLeftPos = position + new Vector2(-offset, offset);
        topRightPos = position + new Vector2(offset, offset);
        botRightPos = position + new Vector2(offset, -offset);
        botLeftPos = position + new Vector2(-offset, -offset);
    }
}
