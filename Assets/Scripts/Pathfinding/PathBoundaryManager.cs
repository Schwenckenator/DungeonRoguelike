using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridPathfinding;

public class PathBoundaryManager : MonoBehaviour
{
    private static PathBoundaryManager Instance { get; set; }
    public MeshFilter[] meshes;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public static PathNode[] SortNodes(PathNode[] nodes) {
        List<PathNode> sortedNodes = new List<PathNode>();
        Debug.Log("Sorting set of nodes");
        //Sort by x: 0 -> inf, then y: 0 -> inf
        foreach (var node in nodes) {
            
            int index = 0;
            foreach(var sortedNode in sortedNodes) {
                // Is y postion before sorted?
                if(node.position.y < sortedNode.position.y) {
                    break;
                } else if (node.position.y == sortedNode.position.y){
                    //Check x
                    if(node.position.x < sortedNode.position.x) {
                        break;
                    }
                }
                index++;
            }
            Debug.Log($"Node {node} sorted into index {index}");
            sortedNodes.Insert(index, node);
        }

        //throw new System.Exception("Not done yet stooge.");
        Debug.Log("Printing set of nodes!");
        for(int i=0; i<sortedNodes.Count; i++) {
            Debug.Log($"Node: {sortedNodes[i]} at index {i}");
        }


        return sortedNodes.ToArray();
    }

    public static void SetupBoundaries(PathNode[] oneMove, PathNode[] maxMove) {
        Vector3[] vertices = new Vector3[oneMove.Length];
        List<Vector3> triangles = new List<Vector3>();

        throw new System.Exception("Not done yet stooge.");

        for (int i = 0; i < oneMove.Length; i++) {
            vertices[i] = oneMove[i].position.ToVector3Int();

            Mesh mesh = new Mesh();
            mesh.name = "OneMove";
            mesh.vertices = vertices;

            
        }
    }


}
