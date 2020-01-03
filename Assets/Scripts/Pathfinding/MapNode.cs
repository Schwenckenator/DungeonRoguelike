using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public bool IsPathable { get; set; }
    public int Cost { get; set; }


    public MapNode(bool isPathable, int cost) {
        IsPathable = isPathable;
        Cost = cost;
    }

    public override string ToString() {
        return $"Pathnode: IsPathable {IsPathable}; Cost {Cost}.";
    }
}
