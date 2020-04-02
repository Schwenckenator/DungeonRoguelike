using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EffectValueBundle
{
    public Effect effect;
    public int minValue;
    public int maxValue;

    public EffectValueBundle(Effect effect, int minValue, int maxValue) {
        this.effect = effect;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public void TriggerEffect(Entity origin, Entity target) {
        effect.TriggerEffect(origin, target, minValue, maxValue);
    }
}
