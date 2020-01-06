using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SingleTargetAbility", menuName = "Ability/Single Target Ability", order = 51)]
public class SingleTargetAbility : Ability {

    public bool canTargetDead = false;
    public bool canTargetAlive = true;

    public override void TriggerAbility(Entity target) {
        foreach(var effect in effects) {
            effect.TriggerEffect(target, minValue, maxValue);
        }
    }

    public override bool IsLegalTarget(Entity me, Entity[] targets) {

        Entity target = targets[0]; //Single target ability only takes 1 target

        if (!canTargetDead && target.Stats.isDead) return false;
        if (!canTargetAlive && !target.Stats.isDead) return false;

        if(targetType == TargetType.all) {
            return true;
        }
        if (targetType == TargetType.selfOnly) {
            return me == target;
        }
        if (targetType == TargetType.alliesOnly) {
            return me.allegiance == target.allegiance && me != target;
        }
        if (targetType == TargetType.enemiesOnly) {
            return me.allegiance != target.allegiance;
        }
        if (targetType == TargetType.selfAndAllies) {
            return me.allegiance == target.allegiance;
        }
        if (targetType == TargetType.others) {
            return me != target;
        }
        return false;
    }

    public bool IsLegalTarget(Entity me, Entity target) {
        return IsLegalTarget(me, new Entity[] { target });
    }
    //public override void Initialise() {
    //    throw new System.NotImplementedException();
    //}
}
