using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class EntityStats : MonoBehaviour
{
    public float health;
    public float maxHealth;

    public float TestDamage;
    public float TestHealing;

    public Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(float newHealth) {
        //Set Health
        health = newHealth;

        //Check for over/ underflow

        if(health < 0) {
            health = 0;
        }else if (health > maxHealth) {
            health = maxHealth;
        }

        //Update health bar image

        healthBar.fillAmount = (health / maxHealth);
    }
    public void Damage(float damage) {
        float newHealth = health - damage;
        SetHealth(newHealth);
    }
    public void Heal(float healing) {
        float newHealth = health + healing;
        SetHealth(newHealth);
    }
}

[CustomEditor(typeof(EntityStats))]
public class EntityStatsEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        EntityStats myScript = (EntityStats)target;
        if (GUILayout.Button("Set Health")) {
            myScript.SetHealth(myScript.health);
        }
        if (GUILayout.Button("Damage Me!")) {
            myScript.Damage(myScript.TestDamage);
        }
        if (GUILayout.Button("Heal Me!")) {
            myScript.Heal(myScript.TestHealing);
        }
    }
}
