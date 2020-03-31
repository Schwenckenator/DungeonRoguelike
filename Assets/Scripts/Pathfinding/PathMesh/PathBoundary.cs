using GridPathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBoundary : MonoBehaviour
{
    public MeshFilter meshFilter;
    public GameObject lineObject;

    public List<LineRenderer> lineRenderers;
    //public LineRenderer lineRenderer;
    public int size = 100; // Size of the entire map

    private Voxel2D[] voxels;
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private List<Edge2D> edges;
    private List<Edge2D> lonelyEdges;
    private List<List<Edge2D>> connectedEdges;
    

    public void Initialise() {
        voxels = new Voxel2D[size * size];
        mesh = new Mesh {
            name = $"MoveMesh-{gameObject.name}"
        };
        vertices = new List<Vector3>();
        triangles = new List<int>();
        edges = new List<Edge2D>();
        lonelyEdges = new List<Edge2D>();
        connectedEdges = new List<List<Edge2D>>();

        for(int i=0, y=0; y < size ; y++) {
            for(int x=0; x < size; x++, i++) {
                voxels[i] = new Voxel2D(x, y);
            }
        }

        meshFilter.sharedMesh = mesh;
    }

    public void Apply(PathNode[] nodes) {
        SetVoxels(nodes);
        Trianglulate();
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
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();
        edges.Clear();
        lonelyEdges.Clear();
        connectedEdges.Clear();

        //TriangulateCellRows();
        TriangulateAllCells();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        
        if(edges.Count > 0) {
            FindNonSharedEdges();
            ConnectEdges();
        }
        SetLinePoints();
    }



    private void TriangulateAllCells() {
        int cells = size - 1;
        for (int i = 0, y = 0; y < cells; y++, i++) {
            for (int x = 0; x < cells; x++, i++) {
                TriangulateSingleCell(voxels[i]);
            }
        }
    }

    private void TriangulateSingleCell(Voxel2D voxel) {
        if (voxel.state) {
            AddQuad(voxel.topLeftPos, voxel.topRightPos, voxel.botRightPos, voxel.botLeftPos);
        }
    }

    private void TriangulateCellRows() {
        int cells = size - 1;
        for(int i=0, y=0; y < cells; y++, i++) {
            for(int x=0; x < cells; x++, i++) {
                TriangulateCell(
                    voxels[i],
                    voxels[i + 1],
                    voxels[i + size],
                    voxels[i + size + 1]
                    );
            }
        }
    }

    private void TriangulateCell(Voxel2D a, Voxel2D b, Voxel2D c, Voxel2D d) {
        int cellType = 0;
        if (a.state) {
            cellType |= 1;
        }
        if (b.state) {
            cellType |= 2;
        }
        if (c.state) {
            cellType |= 4;
        }
        if (d.state) {
            cellType |= 8;
        }

        switch (cellType) {
            case 0:
                return;
                
            // **** Triangles ****
            case 1:
                AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                break;
            case 2:
                AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                break;
            case 4:
                AddTriangle(c.position, c.xEdgePosition, a.yEdgePosition);
                break;
            case 8:
                AddTriangle(d.position, b.yEdgePosition, c.xEdgePosition);
                break;

            // **** Quads ****
            case 3:
                AddQuad(a.position, a.yEdgePosition, b.yEdgePosition, b.position);
                break;
            case 5:
                AddQuad(a.position, c.position, c.xEdgePosition, a.xEdgePosition);
                break;
            case 10:
                AddQuad(a.xEdgePosition, c.xEdgePosition, d.position, b.position);
                break;
            case 12:
                AddQuad(a.yEdgePosition, c.position, d.position, b.yEdgePosition);
                break;
            case 15:
                AddQuad(a.position, c.position, d.position, b.position);
                break;

            // **** Pentagons ****
            case 7:
                AddPentagon(a.position, c.position, c.xEdgePosition, b.yEdgePosition, b.position);
                break;
            case 11:
                AddPentagon(b.position, a.position, a.yEdgePosition, c.xEdgePosition, d.position);
                break;
            case 13:
                AddPentagon(c.position, d.position, b.yEdgePosition, a.xEdgePosition, a.position);
                break;
            case 14:
                AddPentagon(d.position, b.position, a.xEdgePosition, a.yEdgePosition, c.position);
                break;
            
            // **** Corner Cases
            case 6:
                AddTriangle(b.position, a.xEdgePosition, b.yEdgePosition);
                AddTriangle(c.position, c.xEdgePosition, a.yEdgePosition);
                break;
            case 9:
                AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                AddTriangle(d.position, b.yEdgePosition, c.xEdgePosition);
                break;
        }


    }

    private void AddTriangle(Vector3 a, Vector3 b, Vector3 c) {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);

        edges.Add(new Edge2D(a, b));
        edges.Add(new Edge2D(b, c));
        edges.Add(new Edge2D(c, d));
        edges.Add(new Edge2D(d, a));
    }

    private void AddPentagon(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e) {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        vertices.Add(e);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 4);
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
