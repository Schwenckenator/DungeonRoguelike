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
        selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
        selector.transform.localScale = new Vector3(range, range, 1);
        selector.GetComponent<PolygonCollider2D>().points = CalculatePoints(32);
    }

    private Vector2[] CalculatePoints(int arcPoints) {
        float theta;
        Vector2[] points = new Vector2[arcPoints+2];
        points[0] = new Vector2(0, 0); // First point is the origin
        for(int i = 0; i <= arcPoints; i++) {
            theta = (i) * (angle * Mathf.Deg2Rad) / (arcPoints);
            points[i+1] = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
        }
        return points;
    }
}
