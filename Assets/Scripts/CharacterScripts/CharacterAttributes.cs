using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attribute { vitality, might, grace, healthMax, health }

public class CharacterAttributes
{
    private static readonly float vitalityToHealthMult = 10;

    protected Character character;
    readonly Dictionary<Attribute, int> baseAttributes;
    readonly Dictionary<Attribute, List<AttributeModifier>> modifiers;

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

        CalculateAllSecondaryAttributes();
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
        CalculateSecondaryAttributes(attr);
    }
    public void AddModifier(AttributeModifier mod) {
        if(modifiers[mod.attribute].Exists(x => x.id == mod.id)){
            Debug.LogWarning($"{mod.id} already exists!");
            return;
        }
        modifiers[mod.attribute].Add(mod);
        CalculateSecondaryAttributes(mod.attribute);
    }
    public void RemoveModifier(AttributeModifier mod) {
        modifiers[mod.attribute].Remove(mod);
        CalculateSecondaryAttributes(mod.attribute);
    }
    #endregion

    #region private methods
    private void CalculateSecondaryAttributes(Attribute primary) {
        if(primary == Attribute.vitality) {
            // Preserve lost hp when changing maximum
            int hpLost = Get(Attribute.healthMax) - Get(Attribute.health);
            baseAttributes[Attribute.healthMax] = HealthFormula();
            baseAttributes[Attribute.health] = Get(Attribute.healthMax) - hpLost;
        }
    }
    private void CalculateAllSecondaryAttributes() {
        CalculateSecondaryAttributes(Attribute.vitality);
        CalculateSecondaryAttributes(Attribute.might);
        CalculateSecondaryAttributes(Attribute.grace);
    }
    private int HealthFormula() {
        return Mathf.RoundToInt(Get(Attribute.vitality) * vitalityToHealthMult);
    }
    #endregion
}
