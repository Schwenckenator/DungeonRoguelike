using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attribute { vitality, might, grace, healthMax, health }

public class CharacterAttributes
{
    #region Secondary Attribute Formulas
    private int HealthFormula() {
        return Mathf.RoundToInt(Get(Attribute.vitality) * 10);
    }
    #endregion

    protected Character character;
    readonly Dictionary<Attribute, int> baseAttributes;
    readonly Dictionary<Attribute, List<AttributeModifier>> modifiers;
    public Action onAttributeUpdate;
    public Action<float, float> onHealthUpdate;

    public CharacterAttributes(Character character) {
        this.character = character;
        baseAttributes = new Dictionary<Attribute, int>() {
            { Attribute.grace, character.grace },
            { Attribute.might, character.might },
            { Attribute.vitality, character.vitality },
            { Attribute.healthMax, 1 },
            { Attribute.health, 1 }
        };
        modifiers = new Dictionary<Attribute, List<AttributeModifier>>() {
            { Attribute.grace, new List<AttributeModifier>() },
            { Attribute.might, new List<AttributeModifier>() },
            { Attribute.vitality, new List<AttributeModifier>() },
            { Attribute.healthMax, new List<AttributeModifier>() },
            { Attribute.health, new List<AttributeModifier>() }
        };
        
    }
    public void Initialise() {
        CalculateSecondaryAttributes();
    }

    #region public methods
    public int Get(Attribute attr) {
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

    public int GetBase(Attribute attr) {
        return baseAttributes[attr];
    }

    public void SetBase(Attribute attr, int newBase) {
        baseAttributes[attr] = newBase;

        if (attr != Attribute.health) { // Don't waste time calculating for health
            CalculateSecondaryAttributes();
        } else {
            onHealthUpdate?.Invoke(Get(Attribute.health), Get(Attribute.healthMax));
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
        int hpLost = Get(Attribute.healthMax) - Get(Attribute.health);
        baseAttributes[Attribute.healthMax] = HealthFormula();
        baseAttributes[Attribute.health] = Get(Attribute.healthMax) - hpLost;

        //Debug.Log($"Base Max HP is {baseAttributes[Attribute.healthMax]}");

        onAttributeUpdate?.Invoke();
        onHealthUpdate?.Invoke(Get(Attribute.health), Get(Attribute.healthMax));
    }

    #endregion
}
