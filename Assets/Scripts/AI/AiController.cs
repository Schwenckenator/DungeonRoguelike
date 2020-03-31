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

    public AiType aiType;
    //Use this to track patrol boundary
    private Vector2 startingPosition;
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



        aiType = MyEntity.character.aiType;
        
        debugCircle = Instantiate(debugCircle);
        startingPosition = MyEntity.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartTurn() {
        //Reset the turn attempt
        turnAttemptCount = 0;
        Debug.Log("AI TURN! Adding delay...");
        Invoke("DoTurn", 2f);
    }

    private void DoTurn() {
        Debug.Log("Do my turn!");

        Entity nearestEntity = GetNearestEntity(FindTargets());
        //TODO need to check inside nearest target
        //TODO move this hidden check to a better spot
        if(nearestEntity == null) {
            //Do Nothing
            MyEntity.TurnScheduler.actionsRemaining = 0; // Naughty Matt!
            MyEntity.TurnScheduler.ActionFinished();
            return;
        }

        //Move decision making to aiType
        aiType.makeDecision(MyEntity, nearestEntity);


        //TODO turnAttemptCount - maybe this could be based on MyEntity.TurnScheduler.actionsRemaining
        if (turnAttemptCount > 2)
        {
            Debug.Log("Too many turn attempts. Cancelling my turn");

            MyEntity.TurnScheduler.actionsRemaining = 0;
            MyEntity.TurnScheduler.ActionFinished();
        }

        //If there are remaining actions
        if (MyEntity.TurnScheduler.actionsRemaining > 0) {
            Debug.Log("I still have actions, doing turn again.");
            turnAttemptCount++;
            Invoke("DoTurn", 1f);
        } else {
            if (debug) Debug.Log("I have no actions left. Finished Turn.");
        }
    }


    public bool MoveToNearestPlayer(Entity nearestEntity) {
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


            if (debug) Debug.Log("Failed to move to target. Finish Turn.");
            MyEntity.TurnScheduler.actionsRemaining = 0;
            MyEntity.TurnScheduler.ActionFinished();
            return false;
        }

        if (debug) debugCircle.gameObject.transform.position = new Vector3(reachableGoal.x,reachableGoal.y,0);

        float distanceFromGoal = minDistance;

        MyEntity.PathAgent.SetGoalAndFindPath(reachableGoal);


        if (debug) Debug.Log("Finished set goal and find path.");
        return true;

        //MyEntity.ClickToMove.MoveOrder(targetPosition);
        
    }


    public void Attack(Entity targetEntity)
    {

            //You're in range! Punch the sucker!
            if (debug) Debug.Log("Enemy close! Attacking!");
            MyEntity.Interaction.SetCurrentAbility(0); //TODO: Fix Naughty Magic number!
            //Immitate the player using mouse by letting ai show hover.
            MyEntity.Interaction.HoverOverTarget(targetEntity.transform.position);
            MyEntity.Interaction.SelectTarget(targetEntity.transform.position);

    }

    public void Patrol(int maxTiles)
    {
        //Pick a random direction within the maxTiles
        //This is going to get ugly, best not to look
        int direction = Random.Range(0, 3);
        Vector2 target;
        //North
        if (direction == 0)
        {
            target = new Vector2(startingPosition.x+maxTiles,startingPosition.y);
        }
        //East
        else if (direction == 1)
        {
            target = new Vector2(startingPosition.x, startingPosition.y+ maxTiles);
        }
        //South
        else if (direction == 2)
        {
            target = new Vector2(startingPosition.x - maxTiles, startingPosition.y);
        } 
        //West
        else if (direction == 3)
        {
            target = new Vector2(startingPosition.x, startingPosition.y-maxTiles);
        }
        if (debug) Debug.Log("Ai Patrolling");

        //TODO
        //MoveToNearestPlayer()

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
            if (distanceVector.sqrMagnitude < currentSmallestSqrMagnitude && !entity.Stats.isDead && !entity.Stats.isHidden) {
                currentSmallestSqrMagnitude = distanceVector.sqrMagnitude;
                nearestEntity = entity;
            }
        }
        return nearestEntity;
    }
}
