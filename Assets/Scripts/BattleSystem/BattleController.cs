using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BattleController : MonoBehaviour {
    public static BattleController Instance { get; private set; }

    public int CurrentTick { get; private set; }
    public Entity currentEntity;

    public float aggroRadius;

    public bool highlightCombatants = true;

    private List<Turn> turnQueue;
    private bool acceptingNewTurns = true;


    private void Awake() {
        Instance = this;
        turnQueue = new List<Turn>();
        Random.InitState(System.DateTime.Now.Millisecond);
    }
    #region publicMethods

    public void StartBattle() {
        //Search for combatants
        acceptingNewTurns = true;
        //Find combatants
        Entity[] combatants = FindCombatants();
        //All combatants schedule a turn
        foreach(var combatant in combatants) {
            combatant.TurnScheduler.ScheduleTurn();
        }
        //Hand control to first entity
        NextTurn();
    }

    public void EndBattle() {
        acceptingNewTurns = false;
        if (currentEntity != null) {
            currentEntity.TurnScheduler.EndTurn();
        }
        turnQueue.Clear();

        Debug.Log("Battle Finished.");
        CurrentTick = 0;
        currentEntity = null;
    }

    public void ScheduleTurn(Turn newTurn) {
        if (!acceptingNewTurns) {
            Debug.Log("Not accepting new turns.");
            return;
        }
        //Set turn to current tick + delay
        newTurn.SetTick(CurrentTick);
		//Debug.Log($"{newTurn.Entity}'s tick is {newTurn.Tick}. The Current Tick was {CurrentTick}, and the delay was {newTurn.TickDelay}.");

        //Iterate over queue
        //Insert before number bigger than tick

        for (int i = 0; i < turnQueue.Count; i++) {
            if (turnQueue[i].Tick > newTurn.Tick) {
                turnQueue.Insert(i, newTurn);
       //         Debug.Log($"New Turn inserted at index {i}.");
                return;
            }
        }

        turnQueue.Add(newTurn);
    //   Debug.Log("New Turn Added to end.");
    }

    public void NextTurn() {
   //     Debug.Log("Turn Ended, starting next turn.");
        //Disable Control of Current Entity
        /* ***************************************/
        if(currentEntity != null) {
                        
            //Check if any more monsters aggro, if it is a hero
            if (currentEntity.allegiance == EntityAllegiance.player) {
                CheckForNewMonsterAggro();
            }

            currentEntity.TurnScheduler.EndTurn();
        }

        //Find the next turn
        Turn currentTurn = turnQueue[0];
        turnQueue.RemoveAt(0);
  //      Debug.Log($"Next Turn is {currentTurn.ToString()}.");
        //Change current Tick
        CurrentTick = currentTurn.Tick;
  //      Debug.Log($"Current tick changed to {CurrentTick}.");
        //Give control to new Entity
        /* ***************************************/
        currentEntity = currentTurn.Entity;

        PlayerInput.Instance.playerHasControl = (currentEntity.allegiance == EntityAllegiance.player);
        //MainUI.Instance.SetAbilityBar(currentEntity);
        FocusOnUnit.Instance.MoveCameraToUnit(currentEntity.transform);
        currentEntity.TurnScheduler.StartTurn();
    }

    public Entity getCurrentEntity()
    {
        Turn currentTurn = turnQueue[0];

        return currentTurn.Entity;
    }


    public void DebugPrintTurnQueue() {
        int turnCount = 0;
        foreach(Turn turn in turnQueue) {
            Debug.Log($"Turn {turnCount++}, Entity {turn.Entity.ToString()} with Tick {turn.Tick}.");
        }
    }

    #endregion

    #region privateMethods

    /// <summary>
    /// This method is used to find which entities to add to the combat.
    /// </summary>
    private Entity[] FindCombatants() {
        //Find all monsters within aggro radius of a hero
        var newCombatants = new List<Entity>();

        var allPossibleCombatants = FindObjectsOfType<Entity>();
        var heroes = new List<Entity>();

        //Find all heroes
        foreach (var entity in allPossibleCombatants) {
            if(entity.allegiance == EntityAllegiance.player) {
                //It's a hero!
                heroes.Add(entity);
                newCombatants.Add(entity);
            }
        }
        
        //Find all entities within aggro radius of heroes
        foreach(var hero in heroes) {
            var monsters = FindMonstersInRadius(hero);
            foreach(var monster in monsters) {
                if (!newCombatants.Contains(monster)) {
                    newCombatants.Add(monster);
                }
            }
            

            //Collider2D[] hits = Physics2D.OverlapCircleAll(hero.transform.position, aggroRadius);
            //foreach(var hit in hits) {
            //    if (hit.transform == hero.transform) continue; // Don't count yourself
            //    if (!hit.CompareTag("Entity")) continue; // Don't count non-entities
            //    if (hit.GetComponent<Entity>().allegiance == EntityAllegiance.player) continue; //Don't count heroes

            //    //If it's here, it should be a monster!
            //    EntityTurnScheduler monster = hit.GetComponent<EntityTurnScheduler>();
            //    if (!newCombatants.Contains(monster)) {
            //        newCombatants.Add(monster);
            //    }
            //}
        }
        
        
        //Return a list of those entities
        return newCombatants.ToArray();
    }

    private Entity[] FindMonstersInRadius(Entity hero) {
        var monsters = new List<Entity>();
        Collider2D[] hits = Physics2D.OverlapCircleAll(hero.transform.position, aggroRadius);
        foreach (var hit in hits) {
            if (hit.transform == hero.transform) continue; // Don't count yourself
            if (!hit.CompareTag("Entity")) continue; // Don't count non-entities
            if (hit.GetComponent<Entity>().allegiance == EntityAllegiance.player) continue; //Don't count heroes

            //If it's here, it should be a monster!
            Entity monster = hit.GetComponent<Entity>();

            monsters.Add(monster);
        }

        return monsters.ToArray();
    }

    private List<Entity> EntitiesWithTurns() {
        var entitiesWithTurns = new List<Entity>();
        foreach(Turn turn in turnQueue) {
            entitiesWithTurns.Add(turn.Entity);
        }

        return entitiesWithTurns;
    }

    private void CheckForNewMonsterAggro() {
        var monsters = FindMonstersInRadius(currentEntity);
        foreach (var monster in monsters) {
            if (!EntitiesWithTurns().Contains(monster)) {
                monster.TurnScheduler.ScheduleTurn();
            }
        }
    }

    private void OnDrawGizmos() {
        if (highlightCombatants && Application.isPlaying) {
            foreach (var turn in turnQueue) {
                Gizmos.DrawWireSphere(turn.Entity.transform.position, 1.0f);
            }
        }
    }

    #endregion
}

[CustomEditor(typeof(BattleController))]
public class BattleControllerEditor : Editor {
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
