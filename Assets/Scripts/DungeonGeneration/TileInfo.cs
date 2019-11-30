using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileLayer { noCollision, collision }

[System.Serializable]
public class TileInfo
{
    public Color colour;
    public TileBase tile;
    public TileLayer layer;
}
