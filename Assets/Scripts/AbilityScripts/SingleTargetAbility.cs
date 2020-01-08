using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SingleTargetAbility", menuName = "Ability/Single Target Ability", order = 51)]
public class SingleTargetAbility : Ability {

    public override void PrepareSelector(ref GameObject selector) {
        selector.GetComponent<SpriteRenderer>().sprite = selectorSprite;
        selector.transform.localScale = Vector3.one;
        selector.GetComponent<CircleCollider2D>().radius = 0.25f;
    }
}
