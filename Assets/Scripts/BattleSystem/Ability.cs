using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType { self = 1, ally = 2, enemy = 4}
/// <summary>
/// Ability holds valid targets, damage, and all things to do with an interaction
/// </summary>
public abstract class Ability
{
    //Handle type through subclassing
    //I'll think of what all abilities share later.
    public TargetType ValidTargets { get; protected set; }
    public int ActionCost { get; protected set; }
    public float Range { get; protected set; }

    public Ability(TargetType target, int actionCost, float range) {
        ValidTargets = target;
        ActionCost = actionCost;
        Range = range;
    }

    public abstract void Activate(Entity target);
}
