using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class EntityTurnScheduler : MonoBehaviour
{
    public bool debug = true;

    public bool myTurn = false;
    public int myTickDelay = 10;
   // public SpriteRenderer selectionRing;
    //Using object so children are hidden
    public GameObject selectionRingObj;
    //public GameObject clickToMoveObj;

    public int actionsPerGo = 2;
    public int actionsRemaining;

    public Image[] actionArrows;

    private Entity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        // Entities should have 0 actions when it's not their turn

        //actionsRemaining = actionsPerGo;
        actionsRemaining = 0;
        SetActionArrowsVisibility(actionsRemaining);
    }

    // Update is called once per frame
    void Update()
    {
        //Do stuff when myTurn is true


        //Limit turn to number of actions

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
        selectionRingObj.SetActive(false);
        entity.State = EntityState.inactive;

    }

    public void StartTurn() {
        //Say its my turn
        myTurn = true;
        actionsRemaining = actionsPerGo;
        selectionRingObj.SetActive(true);
        entity.State = EntityState.idle;
        entity.ClickToMove.UpdateMaxDistance();

        // Show actions in UI
        SetActionArrowsVisibility(actionsRemaining);

    }

    public void SpendActions(int numberOfActions)
    {
        if (debug) {
            Debug.Log($"Spend actions called, spending {numberOfActions} actions.");
        }

        actionsRemaining -= numberOfActions;
        SetActionArrowsVisibility(actionsRemaining);
        CheckForEndOfTurn();

        if (debug) {
            Debug.Log($"{this.ToString()} has {actionsRemaining} remaining.");
        }
    }

    public void SetActionArrowsVisibility(int actions) {
        for(int i = 0; i < actionArrows.Length; i++) {
            actionArrows[i].enabled = i < actions;
        }
    }

    private void CheckForEndOfTurn() {
        if (actionsRemaining <= 0) {
            //actionsRemaining = actionsPerGo; //Read above comment
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
