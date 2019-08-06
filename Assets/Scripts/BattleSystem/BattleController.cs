using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BattleController : MonoBehaviour {
    public static BattleController Instance { get; private set; }

    public int CurrentTick { get; private set; }
    public EntityTurnScheduler currentEntity;

    private List<Turn> turnQueue;
    

    private void Awake() {
        Instance = this;
        turnQueue = new List<Turn>();
    }

    public void StartBattle() {
        //Search for combatants
        //Just grab all entities for now
        EntityTurnScheduler[] combatants = FindObjectsOfType<EntityTurnScheduler>();
        //All combatants schedule a turn
        foreach(var combatant in combatants) {
            combatant.ScheduleTurn();
        }
        //Hand control to first entity
        NextTurn();
    }

    public void ScheduleTurn(Turn newTurn) {
        //Set turn to current tick + delay
        newTurn.SetTick(CurrentTick);
        Debug.Log($"Turn's tick is {newTurn.Tick}. The Current Tick was {CurrentTick}, and the delay was {newTurn.TickDelay}.");
        //Iterate over queue
        //Insert before number bigger than tick

        for (int i = 0; i < turnQueue.Count; i++) {
            if (turnQueue[i].Tick > newTurn.Tick) {
                turnQueue.Insert(i, newTurn);
                Debug.Log($"New Turn inserted at index {i}.");
                return;
            }
        }

        turnQueue.Add(newTurn);
        Debug.Log("New Turn Added to end.");
    }

    public void NextTurn() {
        Debug.Log("Turn Ended, starting next turn.");
        //Disable Control of Current Entity
        /* ***************************************/
        if(currentEntity != null) {
            currentEntity.EndTurn();
        }

        //Find the next turn
        Turn currentTurn = turnQueue[0];
        turnQueue.RemoveAt(0);
        Debug.Log($"Next Turn is {currentTurn.ToString()}.");
        //Change current Tick
        CurrentTick = currentTurn.Tick;
        Debug.Log($"Current tick changed to {CurrentTick}.");
        //Give control to new Entity
        /* ***************************************/
        currentEntity = currentTurn.Entity;
        currentTurn.Entity.StartTurn();
    }

    public void DebugPrintTurnQueue() {
        int turnCount = 0;
        foreach(Turn turn in turnQueue) {
            Debug.Log($"Turn {turnCount++}, Entity {turn.Entity.ToString()} with Tick {turn.Tick}.");
        }
    }
}

[CustomEditor(typeof(BattleController))]
public class TBattleControllerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        BattleController myScript = (BattleController)target;
        if (Application.isPlaying) {
            if (GUILayout.Button("Start Battle")) {
                myScript.StartBattle();
            }
            if (GUILayout.Button("Next Turn")) {
                myScript.NextTurn();
            }
            if (GUILayout.Button("Print Turn Queue")) {
                myScript.DebugPrintTurnQueue();
            }
        }
    }
}
