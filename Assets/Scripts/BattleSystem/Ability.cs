﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType { all, selfOnly, alliesOnly, enemyOnly, selfAndAllies, others}
public enum AbilityType { empty, damage, heal }
/// <summary>
/// Ability holds valid targets, damage, and all things to do with an interaction
/// </summary>

//[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Ability", order = 51)]
public abstract class Ability : ScriptableObject
{
    //Handle type through subclassing
    //I'll think of what all abilities share later.
    public TargetType targetType;
    public AbilityType abilityType;
    public int actionCost = 2;
    public bool endsTurn = false;
    public float range = 1f;
    public int minValue;
    public int maxValue;

    //public abstract void Initialise();

    public abstract void TriggerAbility(Entity target);
}
