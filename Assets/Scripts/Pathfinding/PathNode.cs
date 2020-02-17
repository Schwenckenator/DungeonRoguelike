using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridPathfinding {
    public class PathNode {
        public int f;
        public int g;
        public int h;
        public int score;
        public int distance;
        public int stepCost;

        public PathNode parent;
        public Vector2Int position;

        public PathNode(PathNode parent, Vector2Int position, int stepCost = 10) {
            this.parent = parent;
            this.position = position;
            this.stepCost = stepCost;
        }

        public override string ToString() {
            return $"Pathnode {position.ToString()}";
        }

        public override bool Equals(object obj) {

            if (obj == null || !this.GetType().Equals(obj.GetType())) {
                return false;
            } else {
                PathNode p = (PathNode)obj;
                return position == p.position;
            }
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
