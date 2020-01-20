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

    private Dictionary<string, int> attributes;

    public bool isDead = false;

    //public float TestDamage;
    //public float TestHealing;

    public Image healthBar;
    public TextMeshProUGUI healthText;

    private Entity myEntity;

    public void Initialise() {
        myEntity = GetComponent<Entity>();
        attributes = myEntity.character.GetAttributes();
        SetHealth(attributes["hp_max"]);
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
        attributes["hp_now"] = newHealth;

        //Check for over/ underflow

        if(attributes["hp_now"] < 0) {
            attributes["hp_now"] = 0;
            Die();
        }else if (attributes["hp_now"] > attributes["hp_max"]) {
            attributes["hp_now"] = attributes["hp_max"];
        }

        //Update health bar image

        healthBar.fillAmount = (attributes["hp_now"] / attributes["hp_max"]);
        healthText.text = $"{attributes["hp_now"]} / {attributes["hp_max"]}";
    }

    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    public void ModifyHealth(int value) {
        int newHealth = attributes["hp_now"] + value;
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
