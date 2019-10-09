﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A rectangular shaped room.
/// </summary>
public class Room
{

    public Vector2Int Centre { get; }

    public Vector2Int Size { get; }
    // TODO: check below
    // Will this get weird with odd-number sized numbers?
    public int Width { get
        {
            return Size.x;
        } }
    public int Height { get
        {
            return Size.y;
        } }
    public BoundsInt Bounds { get; }

    public List<Room> Neighbours { get; }
    public int ConnectionCount
    {
        get
        {
            return Neighbours.Count;
        }
    }

    //Constructor
    public Room(Vector2Int centre, Vector2Int size) {
        Centre = centre;
        Size = size;

        Vector2Int min = centre - (new Vector2Int(size.x / 2, size.y / 2));

        Bounds = new BoundsInt((Vector3Int)min, (Vector3Int)size);
        Neighbours = new List<Room>();
    }

    public void Connect(Room other) {
        Neighbours.Add(other);
        other.Neighbours.Add(this);
    }

    public float Distance(Room other) {
        return Distance(this, other);
    }

    public float SqrDistance(Room other) {
        return SqrDistance(this, other);
    }

    public static float Distance(Room value1, Room value2) {
        return (value1.Centre - value2.Centre).magnitude;
    }

    public static float SqrDistance(Room value1, Room value2) {
        return (value1.Centre - value2.Centre).sqrMagnitude;
    }

    public void DebugDataDump() {
        Debug.Log(
            $"Room: {Centre.ToString()}\nSize: {Size.ToString()}\nBounds: {Bounds.ToString()}" +
            $"Min X: {Bounds.xMin}, Max X: {Bounds.xMax}, Min Y: {Bounds.yMin}, Max Y:{Bounds.yMax}");
    }
}
