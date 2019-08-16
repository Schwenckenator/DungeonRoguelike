using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

public class EntityStats : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public bool isDead = false;

    //public float TestDamage;
    //public float TestHealing;

    public Image healthBar;

    private Entity myEntity;

    public void Initialise() {
        myEntity = GetComponent<Entity>();
    }

    public void SetHealth(float newHealth) {
        //Set Health
        health = newHealth;

        //Check for over/ underflow

        if(health < 0) {
            health = 0;
            Die();
        }else if (health > maxHealth) {
            health = maxHealth;
        }

        //Update health bar image

        healthBar.fillAmount = (health / maxHealth);
    }

    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    public void ModifyHealth(float value) {
        float newHealth = health + value;
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
