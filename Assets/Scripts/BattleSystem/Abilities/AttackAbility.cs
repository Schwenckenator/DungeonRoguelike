//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AttackAbility : Ability {
//    public int MinDamage { get; protected set; }
//    public int MaxDamage { get; protected set; }

//    public AttackAbility(TargetType target, int actionCost, float range, int minDamage, int maxDamage) : base(target, actionCost, range) {
//        MinDamage = minDamage;
//        MaxDamage = maxDamage;
//    }

//    public override void TriggerAbility(Entity target) {
//        target.Stats.Damage(GetDamage());
//    }

//    private float GetDamage() {
//        return Random.Range(MinDamage, MaxDamage + 1);
//    }
//}
