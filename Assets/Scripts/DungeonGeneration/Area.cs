using System.Collections;
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
        Debug.Log($"Checking if area filled, min {minX},{minY}; max {maxX},{maxY}.");

        //DebugDraw.CrossBox(new Vector3(minX, minY, 0), new Vector3(maxX, maxY, 0), Color.red);

        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {

                
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

    public void SetFilled(bool fill, Vector2Int min, Vector2Int max) {
        SetFilled(fill, min.x, min.y, max.x, max.y);
    }

    public void SetFilled(bool fill, Vector2Int position) {
        filled[position.x, position.y] = fill;
    }
}
