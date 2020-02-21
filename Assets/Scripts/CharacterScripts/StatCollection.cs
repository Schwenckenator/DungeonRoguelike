using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType { health, healthMax, mana, vitality, might, grace, }

public class StatCollection
{
    #region Secondary Attribute Formulas
    private int HealthFormula() {
        return Mathf.RoundToInt(Get(StatType.vitality) * 10);
    }
    #endregion

    protected Character character;
    readonly Dictionary<StatType, int> baseAttributes;
    readonly Dictionary<StatType, List<StatModifier>> modifiers;
    public Action onAttributeUpdate;
    public Action<float, float> onHealthUpdate;

    public StatCollection(Character character) {
        this.character = character;
        baseAttributes = new Dictionary<StatType, int>() {
            { StatType.grace, character.grace },
            { StatType.might, character.might },
            { StatType.vitality, character.vitality },
            { StatType.healthMax, 1 },
            { StatType.health, 1 }
        };
        modifiers = new Dictionary<StatType, List<StatModifier>>() {
            { StatType.grace, new List<StatModifier>() },
            { StatType.might, new List<StatModifier>() },
            { StatType.vitality, new List<StatModifier>() },
            { StatType.healthMax, new List<StatModifier>() },
            { StatType.health, new List<StatModifier>() }
        };
        
    }
    public void Initialise() {
        CalculateSecondaryAttributes();
    }

    #region public methods
    public int Get(StatType attr) {
        float total = baseAttributes[attr];
        float multiplier = 1;

        foreach (var modifier in modifiers[attr]) {

            if (modifier.operation == Operation.add) {
                total += modifier.value;
            } else if (modifier.operation == Operation.mult) {
                multiplier += modifier.value;
            }
        }
        total *= multiplier;

        return Mathf.RoundToInt(total);
    }

    public int GetBase(StatType attr) {
        return baseAttributes[attr];
    }

    public void SetBase(StatType attr, int newBase) {
        baseAttributes[attr] = newBase;

        if (attr != StatType.health) { // Don't waste time calculating for health
            CalculateSecondaryAttributes();
        } else {
            onHealthUpdate?.Invoke(Get(StatType.health), Get(StatType.healthMax));
        }
    }

    public void AddModifier(StatModifier mod) {
        if(modifiers[mod.attribute].Exists(x => x.id == mod.id)){
            Debug.LogWarning($"{mod.id} already exists!");
            return;
        }
        modifiers[mod.attribute].Add(mod);
        CalculateSecondaryAttributes();
    }

    public void RemoveModifier(StatModifier mod) {
        modifiers[mod.attribute].Remove(mod);
        CalculateSecondaryAttributes();
    }
    
    #endregion

    #region private methods
    private void CalculateSecondaryAttributes() {

        //Vitality
        // Preserve lost hp when changing maximum
        int hpLost = Get(StatType.healthMax) - Get(StatType.health);
        baseAttributes[StatType.healthMax] = HealthFormula();
        baseAttributes[StatType.health] = Get(StatType.healthMax) - hpLost;

        //Debug.Log($"Base Max HP is {baseAttributes[Attribute.healthMax]}");

        onAttributeUpdate?.Invoke();
        onHealthUpdate?.Invoke(Get(StatType.health), Get(StatType.healthMax));
    }

    #endregion
}
