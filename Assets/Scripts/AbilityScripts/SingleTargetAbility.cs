using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SingleTargetAbility", menuName = "Ability/Single Target Ability", order = 51)]
public class SingleTargetAbility : Ability {

    //private readonly Vector2[] points = {
    //    new Vector2(0.25f, 0f),
    //    new Vector2(0f, 0.25f),
    //    new Vector2(-0.25f, 0f),
    //    new Vector2(0f, -0.25f)
    //};

    public override void DisplayVisual(Entity me)
    {
        Vector2 position = me.transform.position;
        Instantiate(visual, position, Quaternion.identity);
    }

    public override void PrepareSelector(ref GameObject selector) {
        //selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
        PositionLocked = false;
        selector.transform.localScale = Vector3.one * 0.95f; // Need it smaller than 1 or it splashes
        var points = CalculateCirclePoints(16);
        selector.GetComponent<PolygonCollider2D>().points = points.ToVector2s();
        selector.GetComponent<MeshFilter>().mesh = CreateMesh(points, "Circle");
    }

    private Vector3[] CalculateCirclePoints(int totalPoints) {
        float theta = 0;
        Vector3[] points = new Vector3[totalPoints];
        for (int i = 0; i < totalPoints; i++) {
            theta = i * (2 * Mathf.PI) / totalPoints;
            points[i] = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) / 2;
        }
        return points;
    }
}
