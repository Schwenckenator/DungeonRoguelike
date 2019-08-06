﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EntityTurnScheduler : MonoBehaviour
{
    public bool myTurn = false;
    public int myTickDelay = 10;
    public SpriteRenderer selectionRing;
    public GameObject clickToMoveObj;
   
    public int actionsPerGo = 2;
    public int actionsRemaining;

    // Start is called before the first frame update
    void Start()
    {
        actionsRemaining = actionsPerGo;

    }

    // Update is called once per frame
    void Update()
    {
        //Do stuff when myTurn is true


        //Limit turn to number of actions
        if (actionsRemaining <= 0)
        {
            EndTurn();
        }
    }

    public void ScheduleTurn(int tickDelay) {
        
        Turn myNextTurn = new Turn(this, tickDelay);

        BattleController.Instance.ScheduleTurn(myNextTurn);
    }
    public void ScheduleTurn() {
        ScheduleTurn(myTickDelay);
    }

    public void EndTurn() {
        // Schedule next turn in battle
        ScheduleTurn(myTickDelay);

        //Disable self
        myTurn = false;
        selectionRing.enabled = false;
        clickToMoveObj.SetActive(false);


    }

    public void StartTurn() {
        //Say its my turn
        myTurn = true;
        actionsRemaining = actionsPerGo;
        selectionRing.enabled = true;
        clickToMoveObj.SetActive(true);

    }

    public void SpendActions(int numberOfActions)
    {
        actionsPerGo -= numberOfActions;
    }

}

[CustomEditor(typeof(EntityTurnScheduler))]
public class EntityTurnSchedulerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        EntityTurnScheduler myScript = (EntityTurnScheduler)target;
        if (Application.isPlaying) {

            if (GUILayout.Button("Schedule Turn")) {
                myScript.ScheduleTurn(myScript.myTickDelay);
            }

        }
    }
}