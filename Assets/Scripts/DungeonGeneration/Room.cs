using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A rectangular shaped room.
/// </summary>
public class Room {
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

    public Vector2Int Centre { get; }


    private List<Vector2Int> SpawnableSquares { get; }

    //Constructor
    public Room(Vector2Int centre, Vector2Int size) {
        Centre = centre;

        Vector2Int min = new Vector2Int(centre.x - size.x / 2, centre.y - size.y / 2);
        Bounds = new BoundsInt((Vector3Int)min, new Vector3Int(size.x, size.y, 1)); // Zed is 1 for bound checking
        Neighbours = new List<Room>();
        Children = new List<Room>();
        SpawnableSquares = new List<Vector2Int>();
    }

    public void FindSpawnableSquares() {
        Debug.Log("Finding spawnable squares.");
        for (int x = Bounds.xMin; x < Bounds.xMax; x++) {
            for(int y = Bounds.yMin; y < Bounds.yMax; y++) {
                Vector2Int point = new Vector2Int(x, y);
                var hit = Physics2D.OverlapCircle(point, 0.45f, LayerMask.GetMask("Obstacle")); //Not quite a 1 unit diameter circle
                if(hit == null) {
                    SpawnableSquares.Add(point);
                }
            }
        }
    }

    public bool Contains(Vector3Int point) {
        return Bounds.Contains(point);
    }

    public Vector2Int RandomSpawnablePoint() {
        return SpawnableSquares.RandomItem();
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
    public override string ToString() {
        return $"Room: {Bounds.ToString()}";
    }
}
