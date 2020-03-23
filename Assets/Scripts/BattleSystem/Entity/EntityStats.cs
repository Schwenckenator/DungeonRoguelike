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

    public StatCollection Collection { get; private set; }

    public bool isDead = false;

    public Image healthBar;
    public TextMeshProUGUI healthText;
    public Image manaBar;
    public TextMeshProUGUI manaText;

    private Entity myEntity;

    #region public methods
    public void Initialise() {
        myEntity = GetComponent<Entity>();
        Collection = new StatCollection(myEntity.character);
        Collection.onStatUpdate[StatType.health] += UpdateHealthBar;
        Collection.onStatUpdate[StatType.health] += CheckForDeath;
        Collection.onStatUpdate[StatType.mana] += UpdateManaBar;

        Collection.Initialise();
    }

    public void Set(StatType attr, int newValue) {
        Collection.Set(attr, newValue);
    }
    public void ModifyByValue(StatType attr, int value) {
        int newValue = Collection.Get(attr) + value;
        Set(attr, newValue);
    }

    public int Get(StatType attr) {
        return Collection.Get(attr);
    }

    internal void DebugLogStats() {
        Collection.DebugLogStats(myEntity);
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

    private void UpdateManaBar(Stat mana) {
        manaBar.fillAmount = (float)mana.ValueNow / mana.Max;
        manaText.text = $"{mana.ValueNow} / {mana.Max}";
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
