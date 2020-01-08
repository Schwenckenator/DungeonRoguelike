using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Circle Ability", order = 51)]
public class CircleAreaAbility : Ability {

    public float radius;

    public override void PrepareSelector(ref GameObject selector) {
        selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
        selector.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        selector.GetComponent<CircleCollider2D>().radius = 0.5f;
    }
}
