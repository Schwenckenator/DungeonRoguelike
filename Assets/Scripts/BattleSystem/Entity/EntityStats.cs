using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;

public enum Attribute { vitality, might, grace, healthMax }

public class EntityStats : MonoBehaviour
{
    private static readonly float vitalityToHealthMultiplier = 10;

    public int health;
    //private float maxHealth;

    //private CharacterAttributes attributes;
    private Dictionary<Attribute, int> baseAttributes;
    private Dictionary<Attribute, int> modifiedAttributes;
    //private Dictionary<Attribute, List<AttributeModifier>> modifiers;
    

    public bool isDead = false;

    //public float TestDamage;
    //public float TestHealing;

    public Image healthBar;
    public TextMeshProUGUI healthText;

    private Entity myEntity;

    public int TestVitality;

    private void Update() {
        //TestVitality = GetAttribute(Attribute.vitality);
    }

    public void Initialise() {
        myEntity = GetComponent<Entity>();
        //modifiers = new Dictionary<Attribute, List<AttributeModifier>>();

        InitialiseCollections();
        CalculateSecondaryStats();
        
        SetHealth(baseAttributes[Attribute.healthMax]);
        
    }
    private void InitialiseCollections() {
        baseAttributes = new Dictionary<Attribute, int> {
            { Attribute.grace, myEntity.character.grace },
            { Attribute.might, myEntity.character.might },
            { Attribute.vitality, myEntity.character.vitality },
            { Attribute.healthMax, 1 }//Just to Initialise
        };
        modifiedAttributes = new Dictionary<Attribute, int> {
            { Attribute.grace, myEntity.character.grace },
            { Attribute.might, myEntity.character.might },
            { Attribute.vitality, myEntity.character.vitality },
            { Attribute.healthMax, 1 }//Just to Initialise
        };
        //modifiers.Add(Attribute.grace, new List<AttributeModifier>());
        //modifiers.Add(Attribute.might, new List<AttributeModifier>());
        //modifiers.Add(Attribute.vitality, new List<AttributeModifier>());
        //modifiers.Add(Attribute.healthMax, new List<AttributeModifier>());
    }
    private void CalculateSecondaryStats() {
        int hpLost = 
        baseAttributes[Attribute.healthMax] = GetAttribute(Attribute.vitality) * 10;
        UpdateHealthBar();
    }
    public void SetHealth(int newHealth) {
        //Set Health
        health = newHealth;

        //Check for over/ underflow

        if(health < 0) {
            health = 0;
            Die();
        }else if (health > baseAttributes[Attribute.healthMax]) {
            health = baseAttributes[Attribute.healthMax];
        }
        UpdateHealthBar();


    }

    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    private void UpdateHealthBar() {
        //Update health bar image
        //Use float for float division
        float hpMax = baseAttributes[Attribute.healthMax];
        healthBar.fillAmount = (health / hpMax);
        healthText.text = $"{health} / {baseAttributes[Attribute.healthMax]}";
    }

    public void ModifyHealth(int value) {
        int newHealth = health + value;
        SetHealth(newHealth);
    }

    public void AddModifier(AttributeModifier modifier) {
        //modifiers[modifier.attribute].Add(modifier);
        //

        modifiedAttributes[modifier.attribute] += modifier.Operate(baseAttributes[modifier.attribute]);
        CalculateSecondaryStats();
    }
    public void RemoveModifier(AttributeModifier modifier) {
        //modifiers[modifier.attribute].Remove(modifier);
        modifiedAttributes[modifier.attribute] -= modifier.Operate(baseAttributes[modifier.attribute]);
        CalculateSecondaryStats();
    }

    public int GetAttribute(Attribute attribute) {
        //float total = attributes[attribute];
        //float multiplier = 1;

        //foreach(var modifier in modifiers[attribute]) {

        //    if (modifier.operation == Operation.add) {
        //        total += modifier.value;
        //    }
        //    else if (modifier.operation == Operation.mult) {
        //        multiplier += modifier.value;
        //    }
        //}
        //total *= multiplier;

        //return Mathf.RoundToInt(total);
        return modifiedAttributes[attribute];
    }
    public int GetBaseAttribute(Attribute attribute) {
        return baseAttributes[attribute];
    }
}

[CustomEditor(typeof(EntityStats))]
public class EntityStatsEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        //EntityStats myScript = (EntityStats)target;
        //if (GUILayout.Button("Set Health")) {
        //    myScript.SetHealth(myScript.health);
        //}
        ////if (GUILayout.Button("Damage Me!")) {
        ////    myScript.Damage(myScript.TestDamage);
        ////}
        ////if (GUILayout.Button("Heal Me!")) {
        ////    myScript.Heal(myScript.TestHealing);
        ////}
    }
}
