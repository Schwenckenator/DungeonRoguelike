using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Cone Ability", order = 51)]
public class ConeAreaAbility : Ability {

    public float angle = 45;

    public override void DisplayVisual(Vector2 position) {
        Instantiate(visual, position, Quaternion.identity);
    }

    public override void PrepareSelector(ref GameObject selector) {
        selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
        //selector.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        selector.GetComponent<PolygonCollider2D>().points = CalculatePoints();
    }

    private Vector2[] CalculatePoints() {

        throw new System.NotImplementedException();
    }
}
