using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A doubly linked positional node
/// </summary>
public class PositionNode
{
    public Vector2Int position;
    public PositionNode parent;
    public PositionNode child;

    public PositionNode(Vector2Int position) {
        this.position = position;
    }

    public override bool Equals(object obj) {

        if (obj == null || !GetType().Equals(obj.GetType())) {
            return false;
        } else {
            PositionNode p = (PositionNode)obj;
            return position == p.position;
        }
    }
    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
