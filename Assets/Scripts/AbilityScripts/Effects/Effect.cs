using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the effect of abilites. 
/// </summary>
/// 

public abstract class Effect : ScriptableObject {
    public abstract void TriggerEffect(Entity target, int minValue, int maxValue);
}
