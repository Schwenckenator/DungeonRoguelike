using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public Vector2Int Centre { get; }
    public List<Vector2Int> Neighbours { get; }
    public int ConnectionCount
    {
        get
        {
            return Neighbours.Count;
        }
    }

    public Room(Vector2Int centre) {
        Centre = centre;
        Neighbours = new List<Vector2Int>();
    }
}
