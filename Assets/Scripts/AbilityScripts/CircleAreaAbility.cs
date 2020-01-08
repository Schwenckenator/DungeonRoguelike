using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Circle Ability", order = 51)]
public class CircleAreaAbility : Ability {

    public float radius;

    public override GameObject PrepareSelector() {
        selector.transform.localScale = new Vector2(radius * 2, radius * 2);
        return selector;
    }

}
