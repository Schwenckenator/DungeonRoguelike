using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode: IComparable{
    public int f;
    public int g;
    public int h;

    public PathNode parent;
    public Vector2Int position;

    public PathNode(PathNode parent, Vector2Int position) {
        this.parent = parent;
        this.position = position;
    }

    public int CompareTo(object obj) {
        return f.CompareTo(obj);
    }

    public override string ToString() {
        return $"Pathnode {position.ToString()}";
    }

    public override bool Equals(object obj) {

        if(obj == null || !this.GetType().Equals(obj.GetType())) {
            return false;
        } else {
            PathNode p = (PathNode) obj;
            return position == p.position;
        }        
    }
    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
