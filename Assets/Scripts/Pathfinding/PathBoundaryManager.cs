using GridPathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathBoundaryManager : MonoBehaviour
{
    private static PathBoundaryManager Instance { get; set; }
    public MeshFilter[] meshFilters;

    private Mesh[] meshes;

    static Dictionary<Vector2, int> nodeIndexDict;

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        nodeIndexDict = new Dictionary<Vector2, int>();
        meshes = new Mesh[meshFilters.Length];
        for(int i=0; i< meshFilters.Length; i++) {
            meshes[i] = new Mesh {
                name = $"MoveMesh{i}"
            };
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
            //Debug.Log($"Node {node} sorted into index {index}");
            sortedNodes.Insert(index, node);
        }

        //throw new System.Exception("Not done yet stooge.");
        //Debug.Log("Printing set of nodes!");
        //for(int i=0; i<sortedNodes.Count; i++) {
        //    Debug.Log($"Node: {sortedNodes[i]} at index {i}");
        //}


        return sortedNodes.ToArray();
    }

    public static void SetupBoundaries(PathNode[] oneMove, PathNode[] maxMove) {

        Instance.meshes[0] = SetupBoundary(oneMove, Instance.meshes[0]);
        Instance.meshFilters[0].sharedMesh = Instance.meshes[0];

        //Instance.meshes[1] = SetupBoundary(maxMove, Instance.meshes[1]);
        //Instance.meshFilters[1].sharedMesh = Instance.meshes[1];

    }

    private static Mesh SetupBoundary(PathNode[] nodes, Mesh mesh) {
        var nodePositions = new List<Vector2>();
        nodeIndexDict.Clear();
        var vertices = new List<Vector3>();
        var tris = new List<int>();


        // Pull only positions from nodes
        //nodePositions = nodes.Select(x => x.position).ToList();
        foreach(var node in nodes) {
            nodePositions.Add(new Vector2(node.position.x - 0.45f, node.position.y - 0.45f));
            nodePositions.Add(new Vector2(node.position.x + 0.45f, node.position.y - 0.45f));
            nodePositions.Add(new Vector2(node.position.x - 0.45f, node.position.y + 0.45f));
            nodePositions.Add(new Vector2(node.position.x + 0.45f, node.position.y + 0.45f));
        }

        // Populate Dictionary with Vectors and their indices for easy checking
        for (int i = 0; i < nodePositions.Count; i++) {
            nodeIndexDict.Add(nodePositions[i], i);
        }

        //Setup triangles
        for (int i = 0; i < nodePositions.Count; i++) {
            float x = nodePositions[i].x;
            float y = nodePositions[i].y;

            // Is there not a vertex behind us?
            if (!Contains(x-1, y)) {
                // There isn't
                
                if(Contains(x-1, y+1) && Contains(x, y+1)) {
                    // Make triange behind us
                    tris.Add(GetPos(x, y));
                    tris.Add(GetPos(x - 1, y + 1));
                    tris.Add(GetPos(x, y + 1));
                }
            }

            //Do we have a complete quad?
            if(Contains(x, y+1) && Contains(x+1, y+1) && Contains(x+1, y)) {
                //Yes!
                tris.Add(GetPos(x, y));
                tris.Add(GetPos(x, y + 1));
                tris.Add(GetPos(x + 1, y + 1));

                tris.Add(GetPos(x, y));
                tris.Add(GetPos(x + 1, y + 1));
                tris.Add(GetPos(x + 1, y));
            } else {
                // Missing next x
                if (Contains(x, y + 1) && Contains(x + 1, y + 1)) {
                    tris.Add(GetPos(x, y));
                    tris.Add(GetPos(x, y + 1));
                    tris.Add(GetPos(x + 1, y + 1));
                }
                // Missing next y
                if (Contains(x + 1, y + 1) && Contains(x + 1, y)) {
                    tris.Add(GetPos(x, y));
                    tris.Add(GetPos(x + 1, y + 1));
                    tris.Add(GetPos(x + 1, y));
                }
                //Missing y diagonal
                if (Contains(x, y + 1) && Contains(x + 1, y)) {
                    tris.Add(GetPos(x, y));
                    tris.Add(GetPos(x, y + 1));
                    tris.Add(GetPos(x + 1, y));
                }
            }
        }

        //vertices = nodePositions.Select(x => x.position).ToList()
        vertices.AddRange(nodePositions.Select(x => x.ToVector3()));
        Vector2[] uvs = new Vector2[vertices.Count];

        for (int i = 1; i < uvs.Length; i++) {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        mesh.SetVertices(vertices);
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs;

        return mesh;
    }

    private static bool Contains(float x, float y) {
        return nodeIndexDict.ContainsKey(new Vector2(x, y));
    }

    private static bool TryGetPos(float x, float y, out int index) {
        return nodeIndexDict.TryGetValue(new Vector2(x, y), out index);
    }

    private static int GetPos(float x, float y) {
        return nodeIndexDict[new Vector2(x, y)];
    }

}
