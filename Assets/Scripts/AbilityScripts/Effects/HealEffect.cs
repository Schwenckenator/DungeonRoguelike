using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealEffect", menuName = "Effect/Heal Effect", order = 52)]
public class HealEffect : Effect {

    public override void TriggerEffect(Entity[] targets, int minValue, int maxValue) {
        foreach(Entity target in targets) {
            target.Stats.ModifyHealth(Random.Range(minValue, maxValue + 1));
        }
    }
}
