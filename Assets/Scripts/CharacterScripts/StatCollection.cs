using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType { health, mana, vitality, might, grace, }

public class StatCollection
{
    #region Secondary Attribute Formulas
    private int HealthFormula() {
        return Mathf.RoundToInt(Get(StatType.vitality) * 10);
    }
    #endregion

    protected Character character;
    readonly Dictionary<StatType, Stat> baseAttributes;
    readonly Dictionary<StatType, List<StatModifier>> modifiers;
    public Action onAttributeUpdate;
    public Action<float, float> onHealthUpdate;

    public StatCollection(Character character) {
        this.character = character;
        Stat grace = new Stat("grace", character.grace);
        Stat might = new Stat("might", character.might);
        Stat vitality = new Stat("vitality", character.vitality);
        Stat health = new Stat("health", 1);
        Stat mana = new Stat("mana", 1);

        baseAttributes = new Dictionary<StatType, Stat>() {
            { StatType.grace, grace },
            { StatType.might, might },
            { StatType.vitality, vitality },
            { StatType.health, health },
            { StatType.mana, mana }
        };
        modifiers = new Dictionary<StatType, List<StatModifier>>() {
            { StatType.grace, new List<StatModifier>() },
            { StatType.might, new List<StatModifier>() },
            { StatType.vitality, new List<StatModifier>() },
            { StatType.health, new List<StatModifier>() }
        };
        
    }
    public void Initialise() {
        CalculateSecondaryAttributes();
    }

    #region public methods
    public int Get(StatType attr) {
        float total = baseAttributes[attr].ValueNow;
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
        return baseAttributes[attr].ValueNow;
    }

    public int GetMax(StatType attr) {
        return baseAttributes[attr].Max;
    }

    /// <summary>
    /// Changes the current value and max value. Used for permanent changes.
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="newBase"></param>
    public void SetBase(StatType attr, int newBase) {
        baseAttributes[attr].Value = newBase;

        if (attr != StatType.health) { // Don't waste time calculating for health
            CalculateSecondaryAttributes();
        } else {
            onHealthUpdate?.Invoke(Get(StatType.health), GetMax(StatType.health));
        }
    }

    /// <summary>
    /// Changes the current value only. Used for temporary changes
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="newValue"></param>
    public void Set(StatType attr, int newValue) {

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
        int hpLost = GetMax(StatType.health) - Get(StatType.health);
        baseAttributes[StatType.health].Max = HealthFormula();
        baseAttributes[StatType.health].ValueNow = GetMax(StatType.health) - hpLost;

        //Debug.Log($"Base Max HP is {baseAttributes[Attribute.healthMax]}");

        onAttributeUpdate?.Invoke();
        onHealthUpdate?.Invoke(Get(StatType.health), GetMax(StatType.health));
    }

    #endregion
}
