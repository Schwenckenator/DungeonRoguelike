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

    private StatCollection collection;

    public bool isDead = false;

    public Image healthBar;
    public TextMeshProUGUI healthText;
    public Image manaBar;
    public TextMeshProUGUI manaText;

    private Entity myEntity;

    //TODO Temp idea to test , make private
    private Dictionary<string,GameObject> activeOvertimeEffects;
    //private Dictionary<Item, int> effectOvertime;

    private List<Buff> buffs;

    #region public methods
    public void Initialise() {
        myEntity = GetComponent<Entity>();
        buffs = new List<Buff>();
        activeOvertimeEffects = new Dictionary<string, GameObject>();

        collection = new StatCollection(myEntity.character);
        collection.onStatUpdate[StatType.health] += UpdateHealthBar;
        collection.onStatUpdate[StatType.health] += CheckForDeath;
        collection.onStatUpdate[StatType.mana] += UpdateManaBar;

        collection.Initialise();
    }

    public void Set(StatType attr, int newValue) {
        collection.Set(attr, newValue);
    }
    public void ModifyByValue(StatType attr, int value) {
        int newValue = collection.Get(attr) + value;
        Set(attr, newValue);
    }

    public int Get(StatType attr) {
        return collection.Get(attr);
    }

    public void AddBuff(Buff buff) {
        if (buffs.Exists(x => x.id == buff.id)) {
            if (buff.isStackable) {
                //Add stacking code here?
                Debug.LogWarning($"{buff.id} stacking not implemented!");
                return;
            } else {
                Debug.LogWarning($"{buff.id} already exists!");
                return;
            }
        } 
        
        buffs.Add(buff);
        foreach(StatModifier mod in buff.statModifiers) {
            collection.AddModifier(mod);
        }
    }

    public void RemoveBuff(Buff buff) {
        if (!buffs.Contains(buff)) {
            Debug.LogWarning($"Tried to remove {buff} from buff list but it doesn't exist!");
            return;
        }
        buffs.Remove(buff);
        foreach(StatModifier mod in buff.statModifiers) {
            collection.RemoveModifier(mod);
        }
    }

    public void AddOvertimeEffect(GameObject overTimeEffectObject)
    {
        activeOvertimeEffects[overTimeEffectObject.name] = overTimeEffectObject;
    }
    public bool CheckForOverTimeEffect(string overTimeEffectObjectName)
    {
        return true;
    }

    internal void DebugLogStats() {
        collection.DebugLogStats(myEntity);
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
