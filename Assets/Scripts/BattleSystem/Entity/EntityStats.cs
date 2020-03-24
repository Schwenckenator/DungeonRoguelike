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
    private List<Effect> activeOvertimeEffects;

    #region public methods
    public void Initialise() {
        myEntity = GetComponent<Entity>();
        stats = new StatCollection(myEntity.character);
        //stats.onHealthUpdate += UpdateHealthBar;
        stats.onStatUpdate[StatType.health] += UpdateHealthBar;
        stats.onStatUpdate[StatType.health] += CheckForDeath;
        stats.Initialise();
    }

    public void SetStat(StatType attr, int newValue) {
        stats.Set(attr, newValue);
    }
    public void ModifyStatByValue(StatType attr, int value) {
        int newValue = stats.Get(attr) + value;
        SetStat(attr, newValue);
    }
    //public void AddOvertimeEffect(Effect effect)
    //{
    //    activeOvertimeEffects.Add(effect);

    //}

    internal void DebugLogStats() {
        stats.DebugLogStats(myEntity);
    }
    #endregion

    #region private methods
    private void CheckForDeath(Stat health) {
        if(health.ValueNow <= 0) {
            isDead = true;
            myEntity.Die();
        }
    }

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

        EntityStats myScript = (EntityStats)target;

        if(GUILayout.Button("Print Stats")) {
            myScript.DebugLogStats();
        }
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
