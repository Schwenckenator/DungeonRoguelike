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

    public StatCollection stats;

    public bool isDead = false;

    public Image healthBar;
    public TextMeshProUGUI healthText;

    private Entity myEntity;

    #region public methods
    public void Initialise() {
        myEntity = GetComponent<Entity>();
        stats = new StatCollection(myEntity.character);
        //stats.onHealthUpdate += UpdateHealthBar;
        stats.onStatUpdate[StatType.health] += UpdateHealthBar;
        stats.Initialise();
    }

    public void SetHealth(int newHealth) {
        stats.Set(StatType.health, newHealth);
    }
    public void ModifyHealth(int value) {
        int newHealth = stats.Get(StatType.health) + value;
        SetHealth(newHealth);
    }
    #endregion

    #region private methods
    private void Die() {
        isDead = true;
        myEntity.Die();
    }

    //private void UpdateHealthBar(float hp, float hpMax) {
    //    healthBar.fillAmount = (hp / hpMax);
    //    healthText.text = $"{hp} / {hpMax}";
    //}

    private void UpdateHealthBar(Stat health) {
        healthBar.fillAmount = (float)health.ValueNow / health.Max;
        healthText.text = $"{health.ValueNow} / {health.Max}";
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
