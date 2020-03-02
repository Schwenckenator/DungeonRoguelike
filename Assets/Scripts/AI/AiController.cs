using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public Entity MyEntity { get; private set; }


    public GameObject debugCircle;

    private int turnAttemptCount = 0;

    public float minDistance = 1;
    public bool debug = true;
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
            MyEntity.TurnScheduler.ActionFinished();
            return;
        }
        float distanceToEntity = (nearestEntity.transform.position - transform.position).magnitude;

        if (distanceToEntity < 1.5f) {
            //You're in range! Punch the sucker!
            if (debug) Debug.Log("Enemy close! Attacking!");

            MyEntity.Interaction.SelectTarget(nearestEntity.transform.position);
           
        } 
        else {
            //Try to move closer to nearest entity.



            if (!MoveToNearestPlayer(nearestEntity))
            {
                if (debug) Debug.Log("Failed to move to target. Finishe Turn.");
                MyEntity.TurnScheduler.actionsRemaining = 0;
                MyEntity.TurnScheduler.ActionFinished();

            }



            //MyEntity.TurnScheduler.actionsRemaining -= 1;
        }

        //If there are remaining actions
        if(MyEntity.TurnScheduler.actionsRemaining > 0) {
            Debug.Log("I still have actions, doing turn again.");
            turnAttemptCount++;
            Invoke("DoTurn", 1f);
        } else {
            if (debug) Debug.Log("I have no actions left. Finished Turn.");
        }
    }

    private bool MoveToNearestPlayer(Entity nearestEntity) {
        if (debug) Debug.Log("MoveToNearestPlayer Called");

        //Move towards target

        //Debug.Log($"My position is {transform.position}, nearest target's position is {nearestEntity.transform.position}.");
        //Debug.Log($"The vector between them is {nearestEntity.transform.position - transform.position}.");
        //Debug.Log($"The vector clamped to magnitude 1 is {Vector3.ClampMagnitude(nearestEntity.transform.position - transform.position, 1f)}");
        //Debug.Log($"The adjacent square vector is {adjacentVector}.");

        Vector2Int origin = new Vector2Int(transform.position.x.RoundToInt(), transform.position.y.RoundToInt());
        Vector2Int goal = new Vector2Int(nearestEntity.transform.position.x.RoundToInt(), nearestEntity.transform.position.y.RoundToInt());

        Vector2Int reachableGoal = MyEntity.PathAgent.GoalToReachableCoord(origin, goal);
        if (reachableGoal == origin)
        {
            //No path found
            return false;
        }

        if (debug) debugCircle.gameObject.transform.position = new Vector3(reachableGoal.x,reachableGoal.y,0);

        float distanceFromGoal = minDistance;

        MyEntity.PathAgent.SetGoalAndFindPath(reachableGoal);


        if (debug) Debug.Log("Finished set goal and find path.");
        return true;

        //MyEntity.ClickToMove.MoveOrder(targetPosition);
        
    }

    //Find the a distance point between two Vectors
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
