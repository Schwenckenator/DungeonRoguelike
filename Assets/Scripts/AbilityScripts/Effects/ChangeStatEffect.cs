using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ChangeStatEffect", menuName = "Effect/Change Stat Effect", order = 52)]
public class ChangeStatEffect : Effect {
    public StatType stat;
    public int multiplier = 1; // Set to 1 or -1 for addition or subraction

    public override void TriggerEffect(Entity origin, Entity target, int minValue, int maxValue) {
        target.Stats.ModifyByValue(stat, Random.Range(minValue, maxValue + 1) * multiplier);
    }
}
