using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public Vector2Int Centre { get; }
    public List<Room> Neighbours { get; }
    public int ConnectionCount
    {
        get
        {
            return Neighbours.Count;
        }
    }

    public Room(Vector2Int centre) {
        Centre = centre;
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
}
