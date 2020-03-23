using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestTurnScheduler : MonoBehaviour
{
    public int tickDelay;
    public Entity entity;

    public void ScheduleTurn() {
        //Turn turn = new Turn(entity, tickDelay);
        //BattleController.Instance.ScheduleTurn(turn);
    }
}

[CustomEditor(typeof(TestTurnScheduler))]
public class TurnSchedulerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TestTurnScheduler myScript = (TestTurnScheduler)target;
        if (Application.isPlaying) {
            if (GUILayout.Button("Schedule Turn")) {
                myScript.ScheduleTurn();
            }
        }
    }
}
