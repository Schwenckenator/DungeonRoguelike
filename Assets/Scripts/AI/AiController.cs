using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public Entity MyEntity { get; private set; }

    private int turnAttemptCount = 0;
    //Basic AI

    //Checks all enemies
    //Finds nearest enemy
    //Moves next to enemy
    //Attacks

    //If no enemies found, ends turn


    // Start is called before the first frame update
    void Start()
    {
        MyEntity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartTurn() {
        Debug.Log("AI TURN! Adding delay...");
        Invoke("DoTurn", 2f);
    }

    private void DoTurn() {
        Debug.Log("Do my turn!");

        Entity nearestEntity = GetNearestEntity(FindTargets());
        if(nearestEntity == null) {
            //Do Nothing
            MyEntity.TurnScheduler.actionsRemaining = 0; // Naughty Matt!
        }
        float distanceToEntity = (nearestEntity.transform.position - transform.position).magnitude;

        //If out of punching range
        if (distanceToEntity > 1.9f) {
            MoveToNearestPlayer(nearestEntity);
        } else {
            Debug.Log("Enemy close! Attacking!");
            //You're in range! Punch the sucker!
            MyEntity.Interaction.SelectTarget(nearestEntity.transform.position);
        }

        //If there are remaining actions
        if(MyEntity.TurnScheduler.actionsRemaining > 0) {
            Debug.Log("I still have actions, doing turn again.");
            turnAttemptCount++;
            Invoke("DoTurn", 1f);
        } else {
            turnAttemptCount = 0;
        }
    }

    private void MoveToNearestPlayer(Entity nearestEntity) {
        Debug.Log("Enemy far away!");
        //Move towards target
        //Find position one square away from target
        Vector3 adjacentVector = nearestEntity.transform.position - transform.position - Vector3.ClampMagnitude(nearestEntity.transform.position - transform.position, 1f);
        
        Debug.Log($"My position is {transform.position}, nearest target's position is {nearestEntity.transform.position}.");
        Debug.Log($"The vector between them is {nearestEntity.transform.position - transform.position}.");
        Debug.Log($"The vector clamped to magnitude 1 is {Vector3.ClampMagnitude(nearestEntity.transform.position - transform.position, 1f)}");
        Debug.Log($"The adjacent square vector is {adjacentVector}.");

        Vector2 adjacentVector2D = new Vector2(adjacentVector.x, adjacentVector.y);
        //Clamp to max range
        Vector2 bestAttemptVector = Vector2.ClampMagnitude(adjacentVector2D, MyEntity.ClickToMove.maxDistanceForOneAction * MyEntity.TurnScheduler.actionsRemaining - turnAttemptCount);

        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y) + bestAttemptVector;

        Debug.DrawLine(transform.position, targetPosition, Color.red, 3f);

        Debug.Log($"Adding Move order to {targetPosition.ToString()}!");

        MyEntity.ClickToMove.MoveOrder(targetPosition);
        
    }

    private static List<Entity> FindTargets() {
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
        List<Entity> possibleTargets = new List<Entity>();

        foreach (var obj in entities) {
            Entity entity = obj.GetComponent<Entity>();
            if (entity.allegiance != EntityAllegiance.monster) {
                possibleTargets.Add(entity);
            }
        }

        return possibleTargets;
    }

    private Entity GetNearestEntity(List<Entity> targets) {
        Entity nearestEntity = null;
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        float currentSmallestSqrMagnitude = Mathf.Infinity;

        foreach (var entity in targets) {
            Vector2 entity2dPos = new Vector2(entity.transform.position.x, entity.transform.position.y);
            Vector2 distanceVector = (entity2dPos - myPos);
            if (distanceVector.sqrMagnitude < currentSmallestSqrMagnitude && !entity.Stats.isDead) {
                currentSmallestSqrMagnitude = distanceVector.sqrMagnitude;
                nearestEntity = entity;
            }
        }
        return nearestEntity;
    }
}
