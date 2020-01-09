using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Circle Ability", order = 51)]
public class CircleAreaAbility : Ability {

    public float radius;

    private readonly Vector2[] points = {
        new Vector2(0.5f, 0f),
        new Vector2(0.462f, 0.191f),
        new Vector2(0.354f, 0.354f),
        new Vector2(0.191f, 0.462f),

        new Vector2(0f, 0.5f),
        new Vector2(-0.191f, 0.462f),
        new Vector2(-0.354f, 0.354f),
        new Vector2(-0.462f, 0.191f),

        new Vector2(-0.5f, 0f),
        new Vector2(-0.462f, -0.191f),
        new Vector2(-0.354f, -0.354f),
        new Vector2(-0.191f, -0.462f),

        new Vector2(0f, -0.5f),
        new Vector2(0.191f, -0.462f),
        new Vector2(0.354f, -0.354f),
        new Vector2(0.462f, -0.191f)
    };

    public override void PrepareSelector(ref GameObject selector) {
        selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
        selector.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        //selector.GetComponent<CircleCollider2D>().radius = 0.5f;
        selector.GetComponent<PolygonCollider2D>().points = points;
    }
}
