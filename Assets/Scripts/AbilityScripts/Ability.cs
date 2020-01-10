using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType { all, selfOnly, alliesOnly, enemiesOnly, selfAndAllies, others}
public enum AbilityType { empty, damage, heal }
/// <summary>
/// Ability holds valid targets, damage, and all things to do with an interaction
/// </summary>

//[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Ability", order = 51)]
public abstract class Ability : ScriptableObject
{
    //Handle type through subclassing
    //I'll think of what all abilities share later.
    public new string name;
    public TargetType targetType;
    public bool canTargetDead = false;
    public bool canTargetAlive = true;
    public Sprite selectorSprite;
    public GameObject visual;

    public Effect[] effects;
    public int actionCost = 1;
    public bool endsTurn = false;
    public bool requireValidTarget = true;
    public float range = 1f;
    public int minValue;
    public int maxValue;

    //public abstract void Initialise();

    public void TriggerAbility(Entity target) {
        foreach (var effect in effects) {
            effect.TriggerEffect(target, minValue, maxValue);
        }
    }
    public abstract void DisplayVisual(Vector2 position);

    public abstract void PrepareSelector(ref GameObject selector);

    public bool IsLegalTarget(Entity me, Entity target) {
        if (!canTargetDead && target.Stats.isDead) return false;
        if (!canTargetAlive && !target.Stats.isDead) return false;

        if (targetType == TargetType.all) {
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
}
