using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the effect of abilites. 
/// </summary>
/// 

    //TODO use event system to track turns taken
public abstract class OTEffect : Effect {
    public abstract void ActivateTriggerEffect(Entity target, int minValue, int maxValue);
    public abstract void DeactivateTriggerEffect(Entity target, int minValue, int maxValue);

}
