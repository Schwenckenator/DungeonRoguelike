using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Shape { circle, square, cone }

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Area Ability", order = 51)]
public class AreaAbility : Ability {

    public override void TriggerAbility(Entity target) {
        throw new System.NotImplementedException();
    }
}
