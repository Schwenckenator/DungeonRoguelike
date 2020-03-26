using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType { all, selfOnly, alliesOnly, enemiesOnly, selfAndAllies, others}

/// <summary>
/// Ability holds valid targets, damage, and all things to do with an interaction
/// </summary>
public abstract class Ability : ScriptableObject, IComparable {

    public new string name;
    public int sortingIndex;
    public TargetType targetType;
    public bool canTargetDead = false;
    public bool canTargetAlive = true;
    public GameObject visual;


    public Effect[] effects;
    public int actionCost = 1;
    public bool endsTurn = false;
    public bool requireValidTarget = true;
    public float range = 1f;
    public int minValue;
    public int maxValue;
    public bool alwaysHit = false;
    public StatType attackStat;
    public StatType defenceStat = StatType.defence;
    public int attackBonus = 0;
    public int manaCost = 0;

    public bool snapToGrid = true;
    public bool isBlockedByTerrain = false;
    public bool PositionLocked { get; protected set; }

    
    public void TriggerAbility(Entity me, Entity target) {
        if (!IsHit(me, target)) {
            Debug.Log("Miss!");
            return;
        } else {
            Debug.Log("Hit!");
        }
        foreach (var effect in effects) {
            effect.TriggerEffect(target, minValue, maxValue);
        }
    }
    public abstract void DisplayVisual(Vector2 postion);

    public abstract void PrepareSelector(ref GameObject selector);

    public bool IsLegalTarget(Entity me, Entity target) {
        if (!canTargetDead && target.Stats.isDead) return false;
        if (!canTargetAlive && !target.Stats.isDead) return false;

        if (targetType == TargetType.all) {
            return true;
        }
        if (targetType == TargetType.selfOnly) {
            return me == target;
        }
        if (targetType == TargetType.alliesOnly) {
            return me.allegiance == target.allegiance && me != target;
        }
        if (targetType == TargetType.enemiesOnly) {
            return me.allegiance != target.allegiance;
        }
        if (targetType == TargetType.selfAndAllies) {
            return me.allegiance == target.allegiance;
        }
        if (targetType == TargetType.others) {
            return me != target;
        }
        return false;
    }

    protected Mesh CreateMesh(Vector3[] points, string meshName = "") {
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();

        vertexList.Add(Vector3.zero); // Add centre point
        vertexList.AddRange(points);

        for (int i = 2; i < points.Length + 1; i++) {
            triangleList.Add(0);
            triangleList.Add(i);
            triangleList.Add(i - 1);
        }
        triangleList.Add(0);
        triangleList.Add(1);
        triangleList.Add(points.Length);

        Vector2[] uvs = new Vector2[vertexList.Count];

        for (int i = 1; i < uvs.Length; i++) {
            uvs[i] = new Vector2(vertexList[i].x, vertexList[i].y);
        }


        Mesh mesh = new Mesh {
            name = meshName,
            vertices = vertexList.ToArray(),
            triangles = triangleList.ToArray(),
            uv = uvs
        };

        return mesh;
    }

    public int CompareTo(object obj) {
        return sortingIndex.CompareTo(obj);
    }

    protected bool IsHit(Entity me, Entity target) {
        if (alwaysHit) return true;

        int baseHitChance = 50;
        int hitChance = baseHitChance + attackBonus + me.Stats.Get(attackStat) - target.Stats.Get(defenceStat);

        if (hitChance >= 100) return true; // if hitchance is 100, don't bother rolling

        int roll = UnityEngine.Random.Range(0, 100); // Rolls 0 - 99
        Debug.Log($"Rolled {roll}, target {hitChance}, diff {hitChance - roll}.");
        return roll < hitChance; 
    }
}
