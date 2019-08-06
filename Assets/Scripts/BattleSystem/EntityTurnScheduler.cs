using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EntityTurnScheduler : MonoBehaviour
{
    public bool myTurn = false;
    public int myTickDelay = 10;
   // public SpriteRenderer selectionRing;
    //Using object so children are hidden
    public GameObject selectionRingObj;
    //public GameObject clickToMoveObj;

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
            actionsRemaining = actionsPerGo;

            BattleController.Instance.NextTurn();

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
        //selectionRing.enabled = false;
        selectionRingObj.SetActive(false);

        GetComponent<ClickToMove>().enabled = false;

        //  GetComponent<ClickToMove>().SetActive(false);
        //GetComponent<ClickToMove>().enabled = false;



    }

    public void StartTurn() {
        //Say its my turn
        myTurn = true;
        actionsRemaining = actionsPerGo;
        selectionRingObj.SetActive(true);

        //selectionRing.enabled = true;
        //  clickToMoveObj.SetActive(true);
        GetComponent<ClickToMove>().enabled = true;

    }

    public void SpendActions(int numberOfActions)
    {
        actionsRemaining -= numberOfActions;
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
