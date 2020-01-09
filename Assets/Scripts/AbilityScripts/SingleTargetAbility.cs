using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SingleTargetAbility", menuName = "Ability/Single Target Ability", order = 51)]
public class SingleTargetAbility : Ability {

    private readonly Vector2[] points = {
        new Vector2(0.25f, 0f),
        new Vector2(0f, 0.25f),
        new Vector2(-0.25f, 0f),
        new Vector2(0f, -0.25f)
    };

    public override void PrepareSelector(ref GameObject selector) {
        selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
        selector.transform.localScale = Vector3.one;
        selector.GetComponent<PolygonCollider2D>().points = points;
        //selector.GetComponent<CircleCollider2D>().radius = 0.25f;
    }
}
