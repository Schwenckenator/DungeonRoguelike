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

    public StatCollection attributes;

    public bool isDead = false;

    public Image healthBar;
    public TextMeshProUGUI healthText;

    private Entity myEntity;

    #region public methods
    public void Initialise() {
        myEntity = GetComponent<Entity>();
        attributes = new StatCollection(myEntity.character);
        attributes.onHealthUpdate += UpdateHealthBar;
        attributes.Initialise();
    }

    public void SetHealth(int newHealth) {
        //Set Health
        int maxHealth = attributes.GetMax(StatType.health);

        //Check for over/ underflow

        if (newHealth < 0) {
            newHealth = 0;
            Die();
        }else if (newHealth > maxHealth) {
            newHealth = maxHealth;
        }
        attributes.Set(StatType.health, newHealth);
        //UpdateHealthBar();


    }
    public void ModifyHealth(int value) {
        int newHealth = attributes.Get(StatType.health) + value;
        SetHealth(newHealth);
    }
    #endregion

    #region private methods
    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    private void UpdateHealthBar(float hp, float hpMax) {
        healthBar.fillAmount = (hp / hpMax);
        healthText.text = $"{hp} / {hpMax}";
    }

    #endregion

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
