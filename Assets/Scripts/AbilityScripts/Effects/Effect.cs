using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the effect of abilites. 
/// </summary>
/// 

public abstract class Effect : ScriptableObject {
    public new string name;

    public abstract void TriggerEffect(Entity[] targets, int minValue, int maxValue);

    public void TriggerEffect(Entity target, int minValue, int maxValue) {
        TriggerEffect(new Entity[] { target }, minValue, maxValue );
    }
    
}
