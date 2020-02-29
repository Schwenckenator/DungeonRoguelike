using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

public class EntityTurnScheduler : MonoBehaviour
{
    public bool debug = true;

    public bool myTurn = false;
    public int myTickDelay = 10;

    public GameObject selectionRingObj;

    public int actionsPerTurn = 2;
    public int actionsRemaining;

    public Image[] actionArrows;

    private Entity myEntity;

    public bool currentlyPerformingAction { get; private set; }

    // Start is called before the first frame update
    public void Initialise()
    {
        myEntity = GetComponent<Entity>();

        // Entities should have 0 actions when it's not their turn
        actionsRemaining = 0;
        SetActionArrowsVisibility(actionsRemaining);
    }

    public void ScheduleTurn(int tickDelay) {
        
        Turn myNextTurn = new Turn(myEntity, tickDelay);

        BattleController.Instance.ScheduleTurn(myNextTurn);
    }
    public void ScheduleTurn() {
        ScheduleTurn(myTickDelay);
    }

    public void StartTurn() {

        if (myEntity.Stats.isDead) {
            //If I'm dead, skip my turn
            BattleController.Instance.NextTurn();
            return;
        }
        MainUI.Instance.SetAbilityBar(myEntity);
        //Say its my turn
        myTurn = true;
        actionsRemaining = actionsPerTurn;
        selectionRingObj.SetActive(true);
        myEntity.State = EntityState.idle;

        // Show actions in UI
        SetActionArrowsVisibility(actionsRemaining);

        GameEvents.current.StartPlayerTurn(gameObject.GetInstanceID());

        if(myEntity.allegiance == EntityAllegiance.monster) {
            GetComponent<AiController>().StartTurn();
        }

    }

    public void EndTurn() {
        if (!myEntity.Stats.isDead) {
            // Schedule next turn in battle if not dead
            ScheduleTurn(myTickDelay);
        }

        //Disable self
        myTurn = false;
        selectionRingObj.SetActive(false);
        myEntity.State = EntityState.inactive;

        GameEvents.current.FinishPlayerTurn(gameObject.GetInstanceID());

    }

    public void SpendActions(int numberOfActions)
    {
        if (debug) {
            Debug.Log($"Spend actions called, spending {numberOfActions} actions.");
        }

        actionsRemaining -= numberOfActions;
        SetActionArrowsVisibility(actionsRemaining);

        myEntity.State = EntityState.idle;

        if (debug) {
            Debug.Log($"{this.ToString()} has {actionsRemaining} remaining.");
        }
    }

    public void SetActionArrowsVisibility(int actions) {
        for(int i = 0; i < actionArrows.Length; i++) {
            actionArrows[i].enabled = i < actions;
        }
    }

    public void ActionStarted() {
        currentlyPerformingAction = true;
    }
    public void ActionFinished() {
        currentlyPerformingAction = false;
        MainUI.Instance.SetAbilityBar(myEntity); //Update ability bar after an action
        CheckForEndOfTurn();
    }

    private void CheckForEndOfTurn() {
        if (actionsRemaining <= 0) {
            actionsRemaining = 0;
            BattleController.Instance.NextTurn();
        }
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

            if (GUILayout.Button("Spend Action")) {
                myScript.SpendActions(1);
            }

        }
    }
}
