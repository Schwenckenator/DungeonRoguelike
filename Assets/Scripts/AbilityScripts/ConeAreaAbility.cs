using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Cone Ability", order = 51)]
public class ConeAreaAbility : Ability {

    public float angle = 90;


    public override void DisplayVisual(Vector2 position) {
        Instantiate(visual, position, Quaternion.identity);
    }

    public override void PrepareSelector(ref GameObject selector) {
        PositionLocked = true;
        selector.transform.localScale = new Vector3(range, range, 1);
        var points = CalculateArcPoints(16);
        selector.GetComponent<PolygonCollider2D>().points = points.ToVector2s();
        selector.GetComponent<MeshFilter>().mesh = CreateMesh(points, "Cone");
    }

    private Vector3[] CalculateArcPoints(int arcPoints) {
        float theta;
        Vector3[] points = new Vector3[arcPoints+2];
        points[0] = new Vector3(0, 0, 0); // First point is the origin
        for(int i = 0; i <= arcPoints; i++) {
            theta = (i - arcPoints/2) * (angle * Mathf.Deg2Rad) / (arcPoints);
            points[i+1] = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
        }
        return points;
    }

}
