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
    public Effect[] effects;
    public int actionCost = 1;
    public bool endsTurn = false;
    public float range = 1f;
    public int minValue;
    public int maxValue;

    //public abstract void Initialise();

    public abstract void TriggerAbility(Entity target);

    public abstract void PrepareAbility();

    //public abstract bool IsLegalTarget(Entity me, Entity[] targets);
}
