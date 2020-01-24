using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;



public class EntityStats : MonoBehaviour
{
    private static readonly float vitalityToHealthMultiplier = 10;

    //public int health;
    //private float maxHealth;

    //private CharacterAttributes attributes;
    //private Dictionary<Attribute, int> baseAttributes;
    //private Dictionary<Attribute, int> modifiedAttributes;
    //private Dictionary<Attribute, List<AttributeModifier>> modifiers;

    public CharacterAttributes attributes;

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
        attributes = new CharacterAttributes(myEntity.character);

        //CalculateSecondaryStats();

        //SetHealth();
        UpdateHealthBar();
    }

    //private void CalculateSecondaryStats() {
    //    int hpLost = 
    //    baseAttributes[Attribute.healthMax] = GetAttribute(Attribute.vitality) * 10;
    //    UpdateHealthBar();
    //}
    public void SetHealth(int newHealth) {
        //Set Health
        int maxHealth = attributes.Get(Attribute.healthMax);

        //Check for over/ underflow

        if (newHealth < 0) {
            newHealth = 0;
            Die();
        }else if (newHealth > maxHealth) {
            newHealth = maxHealth;
        }
        attributes.SetBase(Attribute.health, newHealth);
        UpdateHealthBar();


    }

    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    private void UpdateHealthBar() {
        //Update health bar image
        //Use float for float division
        float hpMax = attributes.Get(Attribute.healthMax);
        float hp = attributes.Get(Attribute.health);
        healthBar.fillAmount = (hp / hpMax);
        healthText.text = $"{hp} / {hpMax}";
    }

    public void ModifyHealth(int value) {
        int newHealth = attributes.Get(Attribute.health) + value;
        SetHealth(newHealth);
    }

    //public void AddModifier(AttributeModifier modifier) {
    //    //modifiers[modifier.attribute].Add(modifier);
    //    //

    //    modifiedAttributes[modifier.attribute] += modifier.Operate(baseAttributes[modifier.attribute]);
    //    CalculateSecondaryStats();
    //}
    //public void RemoveModifier(AttributeModifier modifier) {
    //    //modifiers[modifier.attribute].Remove(modifier);
    //    modifiedAttributes[modifier.attribute] -= modifier.Operate(baseAttributes[modifier.attribute]);
    //    CalculateSecondaryStats();
    //}

    //public int GetAttribute(Attribute attribute) {
    //    //float total = attributes[attribute];
    //    //float multiplier = 1;

    //    //foreach(var modifier in modifiers[attribute]) {

    //    //    if (modifier.operation == Operation.add) {
    //    //        total += modifier.value;
    //    //    }
    //    //    else if (modifier.operation == Operation.mult) {
    //    //        multiplier += modifier.value;
    //    //    }
    //    //}
    //    //total *= multiplier;

    //    //return Mathf.RoundToInt(total);
    //    return modifiedAttributes[attribute];
    //}
    //public int GetBaseAttribute(Attribute attribute) {
    //    return baseAttributes[attribute];
    //}
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
