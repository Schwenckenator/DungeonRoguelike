﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area {
    public readonly int size;
    private bool[,] filled;

    public Area(int size) {
        this.size = size;
        filled = new bool[size, size];
    }

    public bool[,] GetArea() {
        return filled;
    }

    public bool IsFilled(int x, int y) {
        //Squares out of bounds count as filled
        if (x > filled.GetUpperBound(0) ||
            y > filled.GetUpperBound(1) ||
            x < filled.GetLowerBound(0) ||
            y < filled.GetLowerBound(1)) {
            return true;
        }
        return filled[x, y];
    }
    public bool IsFilled(Vector2Int cell) {
        return IsFilled(cell.x, cell.y);
    }

    /// <summary>
    /// Very expensive if it's not filled, I should optimise this
    /// </summary>
    /// <returns>Returns true if any square inside is filled</returns>
    public bool IsFilled(int minX, int minY, int maxX, int maxY) {
        //Debug.Log($"Checking if area filled, min {minX},{minY}; max {maxX},{maxY}.");
        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {

                //Debug.DrawLine(new Vector2(x - 0.5f, y - 0.5f), new Vector2(x + 0.5f, y + 0.5f), Color.red, 3f);
                if (IsFilled(x, y)) return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Very expensive if it's not filled, I should optimise this
    /// </summary>
    /// <returns>Returns true if any square inside is filled</returns>
    public bool IsFilled(Vector2Int min, Vector2Int max) {
        return IsFilled(min.x, min.y, max.x, max.y);
    }

    public void SetFilled(bool fill, int minX, int minY, int maxX, int maxY) {
        if (maxX < minX || maxY < minY) {
            Debug.LogError("MIN is bigger than MAX, swap them around!");
            return;
        }

        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {
                filled[x, y] = fill;
            }
        }
    }
}
