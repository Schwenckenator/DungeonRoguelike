using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge2D
{
    public Vector2 start;
    public Vector2 end;

    public Edge2D (Vector2 start, Vector2 end) {
        this.start = start;
        this.end = end;
    }

    public override bool Equals(object obj) {

        if (obj == null || !GetType().Equals(obj.GetType())) {
            return false;
        } else {
            Edge2D o = (Edge2D)obj;
            return (start == o.start && end == o.end) || (start == o.end && end == o.start);
        }
    }
    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
