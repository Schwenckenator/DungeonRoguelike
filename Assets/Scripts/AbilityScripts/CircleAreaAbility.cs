using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Circle Ability", order = 51)]
public class CircleAreaAbility : Ability {

    public float radius;

    //private readonly Vector2[] points = {
    //    new Vector2(0.5f, 0f),
    //    new Vector2(0.462f, 0.191f),
    //    new Vector2(0.354f, 0.354f),
    //    new Vector2(0.191f, 0.462f),

    //    new Vector2(0f, 0.5f),
    //    new Vector2(-0.191f, 0.462f),
    //    new Vector2(-0.354f, 0.354f),
    //    new Vector2(-0.462f, 0.191f),

    //    new Vector2(-0.5f, 0f),
    //    new Vector2(-0.462f, -0.191f),
    //    new Vector2(-0.354f, -0.354f),
    //    new Vector2(-0.191f, -0.462f),

    //    new Vector2(0f, -0.5f),
    //    new Vector2(0.191f, -0.462f),
    //    new Vector2(0.354f, -0.354f),
    //    new Vector2(0.462f, -0.191f)
    //};

    public override void PrepareSelector(ref GameObject selector) {
        
        selector.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        var points = CalculateCirclePoints(32);
        selector.GetComponent<PolygonCollider2D>().points = points.ToVector2s();
        selector.GetComponent<MeshFilter>().mesh = CalculateMesh(points);
    }

    public override void DisplayVisual(Vector2 position) {
        var newObj = Instantiate(visual, position, Quaternion.identity);
        newObj.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
    }

    private Mesh CalculateMesh(Vector3[] circlePoints) {
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();

        vertexList.Add(Vector3.zero); // Add centre point
        vertexList.AddRange(circlePoints);

        for (int i = 2; i < circlePoints.Length + 1; i++) {
            triangleList.Add(0);
            triangleList.Add(i);
            triangleList.Add(i - 1);
        }
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(circlePoints.Length);

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
        for (int i=0; i<totalPoints; i++) {
            theta = i * (2 * Mathf.PI) / totalPoints;
            points[i] = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) / 2;
        }
        return points;
    }
}
