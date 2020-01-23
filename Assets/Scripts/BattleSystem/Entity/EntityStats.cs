using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;

public enum Attribute { vitality, might, grace, healthMax, healthNow }

public class EntityStats : MonoBehaviour
{
    private static readonly float vitalityToHealthMultiplier = 10;

    //public float health;
    //private float maxHealth;

    //private CharacterAttributes attributes;
    private Dictionary<Attribute, int> attributes;
    private List<AttributeModifier> modifiers;

    public bool isDead = false;

    //public float TestDamage;
    //public float TestHealing;

    public Image healthBar;
    public TextMeshProUGUI healthText;

    private Entity myEntity;

    public void Initialise() {
        myEntity = GetComponent<Entity>();
        //attributes = myEntity.character.attributes;
        //attributes.Initialise();
        InitialiseAttributes();
        CalculateSecondaryStats();
        modifiers = new List<AttributeModifier>();
        SetHealth(attributes[Attribute.healthMax]);
        
    }
    private void InitialiseAttributes() {
        attributes = new Dictionary<Attribute, int> {
            { Attribute.grace, myEntity.character.grace },
            { Attribute.might, myEntity.character.might },
            { Attribute.vitality, myEntity.character.vitality }
        };
    }
    private void CalculateSecondaryStats() {
        attributes[Attribute.healthMax] = attributes[Attribute.vitality] * 10;

    }
    public void SetHealth(int newHealth) {
        //Set Health
        attributes[Attribute.healthNow] = newHealth;

        //Check for over/ underflow

        if(attributes[Attribute.healthNow] < 0) {
            attributes[Attribute.healthNow] = 0;
            Die();
        }else if (attributes[Attribute.healthNow] > attributes[Attribute.healthMax]) {
            attributes[Attribute.healthNow] = attributes[Attribute.healthMax];
        }

        //Update health bar image
        //Use float for float division
        float hpMax = attributes[Attribute.healthMax];
        healthBar.fillAmount = (attributes[Attribute.healthNow] / hpMax);
        healthText.text = $"{attributes[Attribute.healthNow]} / {attributes[Attribute.healthMax]}";
    }

    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    public void ModifyHealth(int value) {
        int newHealth = attributes[Attribute.healthNow] + value;
        SetHealth(newHealth);
    }

    public void AddModifier(AttributeModifier modifier) {
        modifiers.Add(modifier);
        CalculateSecondaryStats();
    }
    public void RemoveModifier(AttributeModifier modifier) {
        modifiers.Remove(modifier);
        CalculateSecondaryStats();
    }

    public int GetAttribute(Attribute attribute) {
        float total = attributes[attribute];
        float multiplier = 1;

        foreach(var modifier in modifiers) {
            if (modifier.attribute != attribute) continue;

            if (modifier.operation == Operation.add) {
                total += modifier.value;
            }
            else if (modifier.operation == Operation.mult) {
                multiplier += modifier.value;
            }
        }
        total *= multiplier;

        return Mathf.RoundToInt(total);
    }
    public int GetBaseAttribute(Attribute attribute) {
        return attributes[attribute];
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
