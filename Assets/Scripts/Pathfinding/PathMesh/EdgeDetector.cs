using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridPathfinding;

public class EdgeDetector
{

    static readonly Vector2Int[] DIRECTIONS = {
                    Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
                    new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1), new Vector2Int(-1,1),
                };

    public static List<PathNode> FindEdges(List<PathNode> nodes) {
        List<PathNode> edgeNodes = new List<PathNode>();
        Debug.Log("Edge Detection: START");
        foreach (var node in nodes) {
            //Check 8 neighbours
            foreach(var dir in DIRECTIONS) {
                if (!nodes.Exists(x => x.position == node.position + dir)){
                    //It's an edge!
                    edgeNodes.Add(node);
                    break;
                }
            }
        }
        Debug.Log("Edge Detection: END");
        Debug.Log("Edge Ordering: START");


        return edgeNodes;
    }
}
