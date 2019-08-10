using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : Ability {
    public int MinDamage { get; private set; }
    public int MaxDamage { get; private set; }

    public AttackAbility(TargetType target, int minDamage, int maxDamage) : base(target) {
        MinDamage = minDamage;
        MaxDamage = maxDamage;
    }

    public override void Activate(Entity target) {
        target.Stats.Damage(GetDamage());
    }

    private float GetDamage() {
        return Random.Range(MinDamage, MaxDamage + 1);
    }
}
