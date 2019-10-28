using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A rectangular shaped room.
/// </summary>
public class Room
{
    // TODO: check below
    // Will this get weird with odd-number sized numbers?
    public BoundsInt Bounds { get; }

    public List<Room> Neighbours { get; }
    public List<Room> Children { get; }
    public int ConnectionCount
    {
        get
        {
            return Neighbours.Count;
        }
    }

    public Vector2Int Centre { get;}

    //Constructor
    public Room(Vector2Int centre, Vector2Int size) {
        Centre = centre;

        Vector2Int min = new Vector2Int(centre.x - size.x / 2, centre.y - size.y / 2);
        Bounds = new BoundsInt((Vector3Int)min, new Vector3Int (size.x, size.y, 1)); // Adds 1 for reasons
        Neighbours = new List<Room>();
        Children = new List<Room>();
    }

    public void Connect(Room other) {
        Neighbours.Add(other);
        Children.Add(other);
        other.Neighbours.Add(this);
    }

    public void Disconnect(Room other) {
        Neighbours.Remove(other);
        Children.Remove(other);
        other.Neighbours.Remove(this);
    }

    public float Distance(Room other) {
        return Distance(this, other);
    }

    public float SqrDistance(Room other) {
        return SqrDistance(this, other);
    }

    public static float Distance(Room value1, Room value2) {
        return (value1.Bounds.center - value2.Bounds.center).magnitude;
    }

    public static float SqrDistance(Room value1, Room value2) {
        return (value1.Centre - value2.Centre).sqrMagnitude;
    }

    public void DebugDataDump() {
        Debug.Log(
            $"Room: {Bounds.center.ToString()}\nSize: {Bounds.size.ToString()}\nBounds: {Bounds.ToString()}" +
            $"Min X: {Bounds.xMin}, Max X: {Bounds.xMax}, Min Y: {Bounds.yMin}, Max Y:{Bounds.yMax}");
    }
}
