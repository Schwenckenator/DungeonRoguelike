using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Increases all stats by a percentage
/// </summary>
public class BlessedCondition : Condition
{
    public StatModifier[] statModifiers;
    private List<StatModifier> appliedMods = new List<StatModifier>();

    protected override void StartCondition(int minValue, int maxValue) {
        base.StartCondition(minValue, maxValue);
        foreach(var mod in statModifiers) {
            StatModifier newMod = mod;
            newMod.value = Random.Range(minValue, maxValue + 1) * 0.01f; // Turn ints into percentage
            appliedMods.Add(newMod);
            target.Stats.Collection.AddModifier(newMod);
        }
    }

    protected override void EndCondition() {
        base.EndCondition();

        foreach (var mod in appliedMods) {
            target.Stats.Collection.RemoveModifier(mod);
        }
        appliedMods.Clear();
    }
}
