using GridPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathBoundaryManager : MonoBehaviour
{
    private static PathBoundaryManager Instance { get; set; }
    public PathBoundary[] pathBoundaries;
    
    static bool ready = false;

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        pathBoundaries[0].Initialise();
        pathBoundaries[1].Initialise();
    }

    //public static PathNode[] SortNodes(PathNode[] nodes) {
    //    List<PathNode> sortedNodes = new List<PathNode>();
    //    Debug.Log("Sorting set of nodes");
    //    //Sort by x: 0 -> inf, then y: 0 -> inf
    //    foreach (var node in nodes) {

    //        int index = 0;
    //        foreach(var sortedNode in sortedNodes) {
    //            // Is y postion before sorted?
    //            if(node.position.y < sortedNode.position.y) {
    //                break;
    //            } else if (node.position.y == sortedNode.position.y){
    //                //Check x
    //                if(node.position.x < sortedNode.position.x) {
    //                    break;
    //                }
    //            }
    //            index++;
    //        }
    //        //Debug.Log($"Node {node} sorted into index {index}");
    //        sortedNodes.Insert(index, node);
    //    }

    //    //throw new System.Exception("Not done yet stooge.");
    //    //Debug.Log("Printing set of nodes!");
    //    //for(int i=0; i<sortedNodes.Count; i++) {
    //    //    Debug.Log($"Node: {sortedNodes[i]} at index {i}");
    //    //}


    //    return sortedNodes.ToArray();
    //}

    public static void SetupBoundaries(PathNode[] oneMove, PathNode[] maxMove) {

        Debug.Log($"One move has {oneMove.Length} nodes.");
        Debug.Log($"Max move has {maxMove.Length} nodes.");
        
        Instance.pathBoundaries[0].Apply(oneMove);
        Instance.pathBoundaries[1].Apply(maxMove);
        //ready = true;
    }

    public static void ClearBoundaries() {
        Instance.pathBoundaries[0].HideLine();
        Instance.pathBoundaries[1].HideLine();
        //ready = true;
    }

}
