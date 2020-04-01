using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Voxel2D
{
    public bool state;

    public Vector2 position, topLeftPos, topRightPos, botRightPos, botLeftPos;

    public Voxel2D(int x, int y) {
        position.x = x;
        position.y = y;

        float offset = 0.5f;

        topLeftPos = position + new Vector2(-offset, offset);
        topRightPos = position + new Vector2(offset, offset);
        botRightPos = position + new Vector2(offset, -offset);
        botLeftPos = position + new Vector2(-offset, -offset);
    }
}
