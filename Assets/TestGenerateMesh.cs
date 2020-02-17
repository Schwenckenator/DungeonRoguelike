using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetCircleMesh", 0.5f);
        //GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void SetCircleMesh() {
        var points = CalculateCirclePoints(32);
        Mesh mesh = CalculateMesh(points);
        //mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<PolygonCollider2D>().points = points.ToVector2s();
    }

    private Mesh CalculateMesh(Vector3[] circlePoints) {
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();

        vertexList.Add(Vector3.zero); // Add centre point
        vertexList.AddRange(circlePoints);

        for (int i = 2; i < circlePoints.Length+1; i++) {
            triangleList.Add(0);
            triangleList.Add(i);
            triangleList.Add(i - 1);

            //triangleList.Add(0);
            //triangleList.Add(i - 1);
            //triangleList.Add(i);
        }
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(circlePoints.Length);

        //triangleList.Add(0);
        //triangleList.Add(circlePoints.Length);
        //triangleList.Add(1);




        Vector2[] uvs = new Vector2[vertexList.Count];
        
        for (int i = 1; i < uvs.Length; i++) {
            uvs[i] = new Vector2(vertexList[i].x, vertexList[i].y);
        }
        

        Mesh mesh = new Mesh {
            name = "Circle",
            vertices = vertexList.ToArray(),
            triangles = triangleList.ToArray(),
            uv = uvs
        };

        return mesh;
    }

    private Vector3[] CalculateCirclePoints(int totalPoints) {
        float theta = 0;
        Vector3[] points = new Vector3[totalPoints];
        //Debug.Log("Creating circle!");
        for (int i = 0; i < totalPoints; i++) {
            theta = i * (2 * Mathf.PI) / totalPoints;
            //Debug.Log($"index {i}, theta {theta}.");
            points[i] = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) / 2;
        }
        //Debug.Log("Circle Finished!");
        return points;
    }
}
