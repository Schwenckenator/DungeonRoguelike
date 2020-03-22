using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BattleController : MonoBehaviour {
    public static BattleController Instance { get; private set; }

    public Entity currentEntity;
    private int entityIndex = 0;
    public EntityAllegiance currentAllegiance;
    

    public bool highlightCombatants = true;

    private List<EntityAllegiance> turnQueue;
    private bool acceptingNewTurns = true;
    private Dictionary<EntityAllegiance, List<Entity>> combatants;

    private void Awake() {
        Instance = this;
        turnQueue = new List<EntityAllegiance>();

        combatants = new Dictionary<EntityAllegiance, List<Entity>>() {
            { EntityAllegiance.hero, new List<Entity>()},
            { EntityAllegiance.monster, new List<Entity>() }
        };

        Random.InitState(System.DateTime.Now.Millisecond);
    }
    #region publicMethods

    public void StartBattle() {
        //Search for combatants
        acceptingNewTurns = true;
        //Find combatants
        Entity[] entities = FindCombatants();
        
        //All found entites get added to the combatants list
        foreach(var entity in entities) {
            combatants[entity.allegiance].Add(entity);
        }
        //Hand control to first entity
        turnQueue.Add(EntityAllegiance.hero);
        //turnQueue.Add(EntityAllegiance.monster);

        currentAllegiance = EntityAllegiance.monster; // Heroes go first, but you need to set it to monster?

        PlayerInput.Instance.onTabPressed += NextEntity;

        NextTurn();
    }

    public void EndBattle() {
        acceptingNewTurns = false;
        if (currentEntity != null) {
            currentEntity.TurnScheduler.EndControl();
        }
        turnQueue.Clear();

        Debug.Log("Battle Finished.");
        currentEntity = null;
    }

    public void ScheduleTurn(EntityAllegiance allegiance) {
        if (!acceptingNewTurns) {
            Debug.Log("Not accepting new turns.");
            return;
        }

        turnQueue.Add(allegiance);
    }

    public void NextTurn() {
        //Disable Control of Current Entity
        if(currentEntity != null) {

            currentEntity.TurnScheduler.EndControl();
        }

        //Add a new turn
        turnQueue.Add(currentAllegiance);

        //Find the next turn
        currentAllegiance = turnQueue[0];
        turnQueue.RemoveAt(0);
        Debug.Log($"Current turn is {currentAllegiance}");

        if(combatants[currentAllegiance].Count == 0) {
            // If there's no combatants, skip turn
            Debug.Log($"No entities in {currentAllegiance}, skipping turn");
            NextTurn();
            return;
        }

        entityIndex = -1; // Next entity adds 1 to make 0;
                
        foreach(Entity entity in combatants[currentAllegiance]) {
            entity.TurnScheduler.Refresh();
        }
        PlayerInput.Instance.playerHasControl = (currentAllegiance == EntityAllegiance.hero);
        NextEntity();
    }

    public void NextEntity() {
        bool found = false;
        if(currentEntity != null) {
            currentEntity.TurnScheduler.EndControl();
        }
        int count = 0;

        while (!found) {
            entityIndex++;
            if (entityIndex > combatants[currentAllegiance].Count - 1) entityIndex = 0;
            Debug.Log($"Next Entity index = {entityIndex}");

            if (combatants[currentAllegiance][entityIndex].TurnScheduler.actionsRemaining > 0) {
                found = true;
            }

            count++;
            if(count > combatants[currentAllegiance].Count - 1) {
                //We've gone through all the entities, and none have any actions left
                Debug.Log("We've gone through all the entities, and none have any actions left");
                NextTurn();
                return;
            }
        }

        currentEntity = combatants[currentAllegiance][entityIndex];
        FocusOnUnit.Instance.MoveCameraToUnit(currentEntity.transform);
        currentEntity.TurnScheduler.StartControl();
    }


    public void DebugPrintTurnQueue() {
        int turnCount = 0;
        foreach (EntityAllegiance turn in turnQueue) {
            Debug.Log($"Turn {turnCount++}, Allegiance {turn.ToString()}");
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
            if(entity.allegiance == EntityAllegiance.hero) {
                //It's a hero!
                heroes.Add(entity);
                newCombatants.Add(entity);
            }
        }
        
        ////Find all entities within aggro radius of heroes
        //foreach(var hero in heroes) {
        //    var monsters = FindMonstersInBounds();
        //    foreach(var monster in monsters) {
        //        if (!newCombatants.Contains(monster)) {
        //            newCombatants.Add(monster);
        //        }
        //    }
           
        //}
        
        
        //Return a list of those entities
        return newCombatants.ToArray();
    }

    //private Entity[] FindMonstersInRadius(Entity hero) {
    //    var monsters = new List<Entity>();
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(hero.transform.position, aggroRadius);
    //    foreach (var hit in hits) {
    //        if (hit.transform == hero.transform) continue; // Don't count yourself
    //        if (!hit.CompareTag("Entity")) continue; // Don't count non-entities
    //        if (hit.GetComponent<Entity>().allegiance == EntityAllegiance.hero) continue; //Don't count heroes

    //        //If it's here, it should be a monster!
    //        Entity monster = hit.GetComponent<Entity>();

    //        monsters.Add(monster);
    //    }

    //    return monsters.ToArray();
    //}

    private Entity[] FindMonstersInBounds(BoundsInt bounds) {
        var monsters = new List<Entity>();
        Collider2D[] hits = Physics2D.OverlapAreaAll(bounds.min.ToVector2Int(), bounds.max.ToVector2Int());

        foreach (var hit in hits) {
            if (!hit.CompareTag("Entity")) continue; // Don't count non-entities
            if (hit.GetComponent<Entity>().allegiance == EntityAllegiance.hero) continue; //Don't count heroes

            //If it's here, it should be a monster!
            Entity monster = hit.GetComponent<Entity>();

            monsters.Add(monster);
        }

        return monsters.ToArray();
    }

    private List<Entity> ActiveCombatants() {
        var activeCombatants = new List<Entity>();
        foreach (var allegianceEntityListPair in combatants) {
            foreach(var entity in allegianceEntityListPair.Value) {
                activeCombatants.Add(entity);
            }
        }

        return activeCombatants;
    }

    //private void CheckForNewMonsterAggro() {
    //    var monsters = FindMonstersInRadius(currentEntity);
    //    foreach (var monster in monsters) {
    //        if (!EntitiesWithTurns().Contains(monster)) {
    //            monster.TurnScheduler.ScheduleTurn();
    //        }
    //    }
    //}

    public void CheckForNewMonsterAggro(BoundsInt bounds) {
        var monsters = FindMonstersInBounds(bounds);
        foreach (var monster in monsters) {
            if (!ActiveCombatants().Contains(monster)) {
                combatants[monster.allegiance].Add(monster);
            }
        }
    }

    private void OnDrawGizmos() {
        //if (highlightCombatants && Application.isPlaying) {
        //    foreach (var turn in turnQueue) {
        //        Gizmos.DrawWireSphere(turn.Entity.transform.position, 1.0f);
        //    }
        //}
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
