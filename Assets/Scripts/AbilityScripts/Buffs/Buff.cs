using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class Buff : ScriptableObject
{
    public string id;
    public bool isStackable = false;
    public StatModifier[] statModifiers;

    public abstract void TriggerBuff(Entity entity);

}
