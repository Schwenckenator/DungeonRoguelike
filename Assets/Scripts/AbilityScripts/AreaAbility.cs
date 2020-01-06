using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AreaAbility", menuName = "Ability/Area Ability", order = 51)]
public class AreaAbility : Ability {

    public override bool IsLegalTarget(Entity me, Entity[] targets) {
        throw new System.NotImplementedException();
    }

    public override void TriggerAbility(Entity target) {
        throw new System.NotImplementedException();
    }
}
