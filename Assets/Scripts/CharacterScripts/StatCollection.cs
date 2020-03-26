using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType { health, mana, defence, armour, grace, intellect, might, vitality }

public class StatCollection
{

    protected Character character;
    readonly Dictionary<StatType, Stat> baseStats;
    readonly Dictionary<StatType, List<StatModifier>> modifiers;
    public Dictionary<StatType, Action<Stat>> onStatUpdate;

    public StatCollection(Character character) {
        this.character = character;
        
        //Primary stats
        Stat grace = new Stat(StatType.grace, character.grace);
        Stat might = new Stat(StatType.might, character.might);
        Stat vitality = new Stat(StatType.vitality, character.vitality);
        Stat intellect = new Stat(StatType.intellect, character.intellect);

        // Secondary Stats
        //TODO: generalise formula
        Stat health = new Stat(StatType.health, 0);
        Stat mana = new Stat(StatType.mana, 0);
        Stat armour = new Stat(StatType.armour, 0);
        Stat defence = new Stat(StatType.defence, 0);


        baseStats = new Dictionary<StatType, Stat>() {
            { StatType.grace,       grace },
            { StatType.intellect,   intellect },
            { StatType.might,       might },
            { StatType.vitality,    vitality },
            { StatType.health,      health },
            { StatType.mana,        mana },
            { StatType.armour,      armour },
            { StatType.defence,     defence }
        };
        modifiers = new Dictionary<StatType, List<StatModifier>>() {
            { StatType.grace,       new List<StatModifier>() },
            { StatType.intellect,   new List<StatModifier>() },
            { StatType.might,       new List<StatModifier>() },
            { StatType.vitality,    new List<StatModifier>() },
            { StatType.health,      new List<StatModifier>() },
            { StatType.mana,        new List<StatModifier>() },
            { StatType.armour,      new List<StatModifier>() },
            { StatType.defence,     new List<StatModifier>() }
        };
        onStatUpdate = new Dictionary<StatType, Action<Stat>>() {
            { StatType.grace, null },
            { StatType.intellect, null },
            { StatType.might, null },
            { StatType.vitality, null },
            { StatType.health, null },
            { StatType.mana, null },
            { StatType.armour, null },
            { StatType.defence, null }
        };

        onStatUpdate[StatType.vitality] += CalculateMaxHealth;
        onStatUpdate[StatType.intellect] += CalculateMaxMana;
        onStatUpdate[StatType.armour] += CalculateDefence;
        onStatUpdate[StatType.grace] += CalculateDefence;
    }



    public void Initialise() {
        foreach (var statActionPair in onStatUpdate) {
            statActionPair.Value?.Invoke(baseStats[statActionPair.Key]);
        }
    }

    #region public methods
    /// <summary>
    /// Gets the stats value with all modifiers applied.
    /// </summary>
    public int Get(StatType attr) {
        float total = baseStats[attr].ValueNow;
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

    /// <summary>
    /// Gets the unmodified stat value.
    /// </summary>
    public int GetBase(StatType attr) {
        return baseStats[attr].ValueNow;
    }

    public int GetMax(StatType attr) {
        return baseStats[attr].Max;
    }

    /// <summary>
    /// Changes the current value and max value. Used for permanent changes.
    /// </summary>
    public void SetBase(StatType attr, int newBase) {
        baseStats[attr].Value = newBase;
        onStatUpdate[attr]?.Invoke(baseStats[attr]);
    }

    /// <summary>
    /// Changes the current value only. Used for temporary changes
    /// </summary>
    public void Set(StatType attr, int newValue) {
        baseStats[attr].ValueNow = newValue;
        onStatUpdate[attr]?.Invoke(baseStats[attr]);
    }

    /// <summary>
    /// Adds a unique modifier to a stat.
    /// </summary>
    public void AddModifier(StatModifier mod) {
        if(modifiers[mod.statType].Exists(x => x.id == mod.id)){
            Debug.LogWarning($"{mod.id} already exists!");
            return;
        }
        modifiers[mod.statType].Add(mod);
        onStatUpdate[mod.statType]?.Invoke(baseStats[mod.statType]);
    }

    /// <summary>
    /// Removes a modifier from a stat.
    /// </summary>
    /// <param name="mod"></param>
    public void RemoveModifier(StatModifier mod) {
        modifiers[mod.statType].Remove(mod);
        onStatUpdate[mod.statType]?.Invoke(baseStats[mod.statType]);
    }

    internal void DebugLogStats(Entity entity) {
        Debug.Log($"Logging Stats for {entity}");
        Debug.Log($"Health: base {GetBase(StatType.health)}, current {Get(StatType.health)}");
        Debug.Log($"Mana: base {GetBase(StatType.mana)}, current {Get(StatType.mana)}");
        Debug.Log($"Grace: base {GetBase(StatType.grace)}, current {Get(StatType.grace)}");
        Debug.Log($"Intellect: base {GetBase(StatType.intellect)}, current {Get(StatType.intellect)}");
        Debug.Log($"Might: base {GetBase(StatType.might)}, current {Get(StatType.might)}");
        Debug.Log($"Vitality: base {GetBase(StatType.vitality)}, current {Get(StatType.vitality)}");
    }

    #endregion

    #region private methods
    //TODO: Generalise these formulas??
    private void CalculateMaxHealth(Stat temp) {
        int mult = 10;
        int hpLost = GetMax(StatType.health) - Get(StatType.health);
        baseStats[StatType.health].Max = Mathf.RoundToInt(Get(StatType.vitality) * mult);
        baseStats[StatType.health].ValueNow = GetMax(StatType.health) - hpLost;

        onStatUpdate[StatType.health]?.Invoke(baseStats[StatType.health]);
    }

    private void CalculateMaxMana(Stat temp) {
        int mult = 10;
        int lost = GetMax(StatType.mana) - Get(StatType.mana);
        baseStats[StatType.mana].Max = Mathf.RoundToInt(Get(StatType.intellect) * mult);
        baseStats[StatType.mana].ValueNow = GetMax(StatType.mana) - lost;

        onStatUpdate[StatType.mana]?.Invoke(baseStats[StatType.mana]);
    }

    private void CalculateDefence(Stat temp) {
        baseStats[StatType.defence].Value = Get(StatType.grace) + Get(StatType.armour);
        onStatUpdate[StatType.defence]?.Invoke(baseStats[StatType.defence]);
    }

    #endregion
}
