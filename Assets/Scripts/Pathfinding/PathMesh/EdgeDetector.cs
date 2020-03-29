using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridPathfinding;

public class EdgeDetector
{

    static readonly Vector2Int[] DIRECTIONS_8 = {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1,1),
    };
    static readonly Vector2Int[] DIRECTIONS_4 = {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static List<PositionNode> FindEdges(Vector2Int origin, List<PositionNode> nodes) {
        List<PositionNode> edgeNodes = new List<PositionNode>();
        Debug.Log("Edge Detection: START");
        foreach (var node in nodes) {
            //Check 8 neighbours
            foreach(var dir in DIRECTIONS_8) {
                if (!nodes.Exists(x => x.position == node.position + dir)){
                    //It's an edge!
                    edgeNodes.Add(node);
                    break;
                }
            }
        }
        Debug.Log("Edge Detection: END");
        Debug.Log("Edge Ordering: START");

        List<PositionNode> orderedEdgeNodes = new List<PositionNode>();
        List<PositionNode> connections = new List<PositionNode>();
        //PositionNode previous = null;
        foreach(var node in edgeNodes) {
            if (node.parent != null) {
                Debug.Log("Already assigned parent");
                continue;
            }
            connections.Clear();
            foreach (var dir in DIRECTIONS_4) {
                if(!edgeNodes.Exists(x => x.position == node.position + dir)) {
                    connections.Add(edgeNodes.Find(x => x.position == node.position + dir));
                }
            }
            Debug.Log($"Connection Count {connections.Count}");
            
        }

        foreach(var node in edgeNodes){
            var currentNode = node;

            if (currentNode.parent == null) {
                //Tis the start
                orderedEdgeNodes.Add(currentNode);
                while(currentNode.child != null) {
                    currentNode = currentNode.child;
                    orderedEdgeNodes.Add(currentNode.child);
                }
            }
        }
        Debug.Log("Edge Ordering: END");
        return orderedEdgeNodes;
    }
}
