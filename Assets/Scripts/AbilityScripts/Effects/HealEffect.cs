using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealEffect", menuName = "Effect/Heal Effect", order = 52)]
public class HealEffect : Effect {

    public override void TriggerEffect(Entity target, int minValue, int maxValue) {
        target.Stats.ModifyStatByValue(StatType.health, Random.Range(minValue, maxValue + 1));
    }
}
