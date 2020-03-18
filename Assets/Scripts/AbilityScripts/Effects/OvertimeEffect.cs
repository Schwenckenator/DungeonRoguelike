using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New OverTime Effect", menuName = "Effect/Overtime Effect 2", order = 52)]
public class OverTimeEffect : OTEffect {
    public int remainingActiveTurns;
    public float hiddenAlpha = 0.5f;
    // Overtime Effect
    public OTEffect oTEffect;

    public override void TriggerEffect(Entity target, int minValue, int maxValue) {
        target.Stats.AddOvertimeEffect(oTEffect);

    }


    public override void ActivateTriggerEffect(Entity target, int minValue, int maxValue)
    {
        throw new System.NotImplementedException();
    }

    public override void DeactivateTriggerEffect(Entity target, int minValue, int maxValue)
    {
        throw new System.NotImplementedException();
    }
}
