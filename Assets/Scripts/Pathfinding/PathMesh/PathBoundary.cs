using GridPathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBoundary : MonoBehaviour
{
    public GameObject lineObject;

    public List<LineRenderer> lineRenderers;
    public int size = 100; // Size of the entire map

    private Voxel2D[] voxels;

    private List<Edge2D> edges;
    private List<Edge2D> lonelyEdges;
    private List<List<Edge2D>> connectedEdges;
    

    public void Initialise() {
        voxels = new Voxel2D[size * size];
        edges = new List<Edge2D>();
        lonelyEdges = new List<Edge2D>();
        connectedEdges = new List<List<Edge2D>>();

        for(int i=0, y=0; y < size ; y++) {
            for(int x=0; x < size; x++, i++) {
                voxels[i] = new Voxel2D(x, y);
            }
        }
    }

    public void Apply(PathNode[] nodes) {
        SetVoxels(nodes);
        Trianglulate();
    }

    public void HideLine() {
        foreach(var renderer in lineRenderers) {
            renderer.enabled = false;
        }
    }

    public void SetVoxels(PathNode[] nodes) {
        //Clear old voxels
        foreach(var voxel in voxels) {
            voxel.state = false;
        }

        foreach(var node in nodes) {
            int index = node.position.x + node.position.y * size;
            voxels[index].state = true;
        }
    }

    private void Trianglulate() {
        edges.Clear();
        lonelyEdges.Clear();
        connectedEdges.Clear();

        EdgifyAllCells();
        
        if(edges.Count > 0) {
            FindNonSharedEdges();
            ConnectEdges();
        }
        SetLinePoints();
    }

    private void EdgifyAllCells() {
        int cells = size - 1;
        for (int i = 0, y = 0; y < cells; y++, i++) {
            for (int x = 0; x < cells; x++, i++) {
                EdgifySingleCell(voxels[i]);
            }
        }
    }

    private void EdgifySingleCell(Voxel2D voxel) {
        if (voxel.state) {
            edges.Add(new Edge2D(voxel.topLeftPos, voxel.topRightPos));
            edges.Add(new Edge2D(voxel.topRightPos, voxel.botRightPos));
            edges.Add(new Edge2D(voxel.botRightPos, voxel.botLeftPos));
            edges.Add(new Edge2D(voxel.botLeftPos, voxel.topLeftPos));
        }
    }

    private void FindNonSharedEdges() {
        Debug.Log($"Edge Count: {edges.Count}");
        foreach (var edge in edges) {
            if (lonelyEdges.Contains(edge)) {
                lonelyEdges.Remove(edge);
            } else {
                lonelyEdges.Add(edge);
            }
        }
        Debug.Log($"Lonely Edges Found: {lonelyEdges.Count}");
    }

    private void ConnectEdges() {

        connectedEdges.Add(new List<Edge2D>());

        int lineIndex = 0;
        connectedEdges[lineIndex].Add(lonelyEdges[0]);
        

        var currentEdge = lonelyEdges[0];

        while (lonelyEdges.Count > 0) {
            bool foundEdge = false;
            lonelyEdges.Remove(currentEdge);

            foreach(var edge in lonelyEdges) {
                if(currentEdge.end == edge.start) {
                    //Found a connection!
                    connectedEdges[lineIndex].Add(edge);
                    currentEdge = edge;
                    foundEdge = true;
                    break;
                }
            }

            if (!foundEdge && lonelyEdges.Count > 0) {
                connectedEdges.Add(new List<Edge2D>());
                lineIndex++;
                currentEdge = lonelyEdges[0];
            }
        }
    }

    private void SetLinePoints() {
        int index = 0;
        foreach(var line in lineRenderers) {
            line.enabled = false;
        }
        Debug.Log($"I have {lineRenderers.Count} renderers, and {connectedEdges.Count} lines to draw");
        foreach(var line in connectedEdges) {
            Debug.Log($"Making line with {line.Count} segments");
            if(index >= lineRenderers.Count) {
                Debug.Log("Index is higher than count, instantiating");
                GameObject obj = Instantiate(lineObject, Vector3.zero, Quaternion.identity ,transform);
                lineRenderers.Add(obj.GetComponent<LineRenderer>());
            }

            List<Vector3> points = new List<Vector3>();

            foreach (var edge in line) {
                points.Add(edge.start);
                points.Add(edge.end);
            }

            lineRenderers[index].enabled = true;
            lineRenderers[index].positionCount = points.Count;
            lineRenderers[index].SetPositions(points.ToArray());
            index++;
        }
    }
}
