using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodPathNode
{

    public int score;
    public FloodPathNode parent;
    public Vector2Int position;

    public FloodPathNode(FloodPathNode parent, Vector2Int position) {
        this.parent = parent;
        this.position = position;
    }

    public override bool Equals(object obj) {

        if (obj == null || !this.GetType().Equals(obj.GetType())) {
            return false;
        } else {
            FloodPathNode p = (FloodPathNode)obj;
            return position == p.position;
        }
    }
    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return $"Node: {position}";
    }
}
