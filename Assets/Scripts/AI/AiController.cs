using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public Entity MyEntity { get; private set; }

    public GameObject debugCircle;

    private int turnAttemptCount = 0;

    public float minDistance = 1;
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

        debugCircle = Instantiate(debugCircle);

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

        if (!MyEntity.PathAgent.PathCheck(transform.position, nearestEntity.transform.position))
        {
            //Do Nothing
            MyEntity.TurnScheduler.actionsRemaining = 0; // Naughty Matt!
        }

        float distanceToEntity = (nearestEntity.transform.position - transform.position).magnitude;

        //If out of punching range
        if (distanceToEntity > 1.9f) {
            //TODO Check for 

            MoveToNearestPlayer(nearestEntity);
            //Moving broken, just skip turn
            MyEntity.TurnScheduler.actionsRemaining = 0;
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
            Debug.Log("I have no actions left.");
            return;
        }
    }

    private void MoveToNearestPlayer(Entity nearestEntity) {
        Debug.Log("MoveToNearestPlayer Called");


        Vector3 adjacentVector = LerpByDistance(nearestEntity.transform.position, transform.position, minDistance);
        Vector2 adjacentVector2D = new Vector2(adjacentVector.x, adjacentVector.y);
        //Debug.Log("Enemy far away!");
        //Move towards target
        //Find position one square away from target
  
        //Debug.Log($"My position is {transform.position}, nearest target's position is {nearestEntity.transform.position}.");
        //Debug.Log($"The vector between them is {nearestEntity.transform.position - transform.position}.");
        //Debug.Log($"The vector clamped to magnitude 1 is {Vector3.ClampMagnitude(nearestEntity.transform.position - transform.position, 1f)}");
        //Debug.Log($"The adjacent square vector is {adjacentVector}.");



        debugCircle.gameObject.transform.position = adjacentVector;
        //TODO need to add a guard to stop going into the negative
        float distanceFromGoal = minDistance;
        //while (!MyEntity.PathAgent.SetGoalAndFindPath(adjacentVector2D))
        if (!MyEntity.PathAgent.SetGoalAndFindPath(adjacentVector2D))
        {
             distanceFromGoal += 0.1f;
             adjacentVector = LerpByDistance(nearestEntity.transform.position, transform.position, minDistance);
             adjacentVector2D = new Vector2(adjacentVector.x, adjacentVector.y);
        }
        Debug.Log("Finished set goal and find path.");

        //Debug.DrawLine(transform.position, adjacentVector2D, Color.red, 10f);

    }


    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
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
