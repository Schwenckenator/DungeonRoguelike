using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : AttackAbility {
    public BasicAttack(TargetType target, int minDamage, int maxDamage) : base(target, minDamage, maxDamage) {
    }

    public override void Activate(Entity target) {
        base.Activate(target);
    }
}
