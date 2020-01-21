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

    //public float health;
    //private float maxHealth;

    private Attributes attributes;

    public bool isDead = false;

    //public float TestDamage;
    //public float TestHealing;

    public Image healthBar;
    public TextMeshProUGUI healthText;

    private Entity myEntity;

    public void Initialise() {
        myEntity = GetComponent<Entity>();
        attributes = myEntity.character.attributes;
        SetHealth(attributes.HealthMax);
    }
    //public void SetMaxHealth(float newMaxHealth) {
    //    if(newMaxHealth < 0) {
    //        newMaxHealth = 0;
    //    }
    //    float healthLost = maxHealth - health;
    //    maxHealth = newMaxHealth;
    //    SetHealth(maxHealth - healthLost);
    //}
    public void SetHealth(int newHealth) {
        //Set Health
        attributes.HealthNow = newHealth;

        //Check for over/ underflow

        if(attributes.HealthNow < 0) {
            attributes.HealthNow = 0;
            Die();
        }else if (attributes.HealthNow > attributes.HealthMax) {
            attributes.HealthNow = attributes.HealthMax;
        }

        //Update health bar image
        //Use float for float division
        float hpMax = attributes.HealthMax;
        healthBar.fillAmount = (attributes.HealthNow / hpMax);
        healthText.text = $"{attributes.HealthNow} / {attributes.HealthMax}";
    }

    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    public void ModifyHealth(int value) {
        int newHealth = attributes.HealthNow + value;
        SetHealth(newHealth);
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
