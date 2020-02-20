using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType { health, healthMax, mana, vitality, might, grace, }

public class AttributeCollection
{
    #region Secondary Attribute Formulas
    private int HealthFormula() {
        return Mathf.RoundToInt(Get(AttributeType.vitality) * 10);
    }
    #endregion

    protected Character character;
    readonly Dictionary<AttributeType, int> baseAttributes;
    readonly Dictionary<AttributeType, List<AttributeModifier>> modifiers;
    public Action onAttributeUpdate;
    public Action<float, float> onHealthUpdate;

    public AttributeCollection(Character character) {
        this.character = character;
        baseAttributes = new Dictionary<AttributeType, int>() {
            { AttributeType.grace, character.grace },
            { AttributeType.might, character.might },
            { AttributeType.vitality, character.vitality },
            { AttributeType.healthMax, 1 },
            { AttributeType.health, 1 }
        };
        modifiers = new Dictionary<AttributeType, List<AttributeModifier>>() {
            { AttributeType.grace, new List<AttributeModifier>() },
            { AttributeType.might, new List<AttributeModifier>() },
            { AttributeType.vitality, new List<AttributeModifier>() },
            { AttributeType.healthMax, new List<AttributeModifier>() },
            { AttributeType.health, new List<AttributeModifier>() }
        };
        
    }
    public void Initialise() {
        CalculateSecondaryAttributes();
    }

    #region public methods
    public int Get(AttributeType attr) {
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

    public int GetBase(AttributeType attr) {
        return baseAttributes[attr];
    }

    public void SetBase(AttributeType attr, int newBase) {
        baseAttributes[attr] = newBase;

        if (attr != AttributeType.health) { // Don't waste time calculating for health
            CalculateSecondaryAttributes();
        } else {
            onHealthUpdate?.Invoke(Get(AttributeType.health), Get(AttributeType.healthMax));
        }
    }

    public void AddModifier(AttributeModifier mod) {
        if(modifiers[mod.attribute].Exists(x => x.id == mod.id)){
            Debug.LogWarning($"{mod.id} already exists!");
            return;
        }
        modifiers[mod.attribute].Add(mod);
        CalculateSecondaryAttributes();
    }

    public void RemoveModifier(AttributeModifier mod) {
        modifiers[mod.attribute].Remove(mod);
        CalculateSecondaryAttributes();
    }
    
    #endregion

    #region private methods
    private void CalculateSecondaryAttributes() {

        //Vitality
        // Preserve lost hp when changing maximum
        int hpLost = Get(AttributeType.healthMax) - Get(AttributeType.health);
        baseAttributes[AttributeType.healthMax] = HealthFormula();
        baseAttributes[AttributeType.health] = Get(AttributeType.healthMax) - hpLost;

        //Debug.Log($"Base Max HP is {baseAttributes[Attribute.healthMax]}");

        onAttributeUpdate?.Invoke();
        onHealthUpdate?.Invoke(Get(AttributeType.health), Get(AttributeType.healthMax));
    }

    #endregion
}
