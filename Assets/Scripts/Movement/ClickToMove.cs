using UnityEngine;
using Pathfinding;
using UnityEngine.EventSystems;

public class ClickToMove : MonoBehaviour
{
    //public bool canMove = false;
    public GameObject moveTarget;
    AIDestinationSetter aiDestination;
    AIPath aiPath;

    public GameObject distanceChecker1;
    public GameObject distanceChecker2;

    private float maxDistanceCurrent;

    //Would like to retrieve this programatically but for short term this works
    public float maxDistance1 = 3f;
    public float maxDistance2 = 6f;
    public float maxDistanceForOneAction = 3f;

    private bool seeking;
    private EntityTurnScheduler turnScheduler;
    private Entity myEntity;
    private Transform target;

    public void Initialise()
    {
  
        aiDestination = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        turnScheduler = GetComponent<EntityTurnScheduler>();
        myEntity = GetComponent<Entity>();

        UpdateMaxDistance();

        //Subscribe from game events
        GameEvents.current.onStartPlayerTurn += OnStartPlayerTurn;
        GameEvents.current.onFinishPlayerTurn += OnFinishPlayerTurn;

    }

    private void OnDestroy()
    {
        //Unsubscribe from game events
        GameEvents.current.onStartPlayerTurn -= OnStartPlayerTurn;
        GameEvents.current.onFinishPlayerTurn -= OnFinishPlayerTurn;
    }

    private void OnEnable() {
        PlayerInput.Instance.onLeftMouseButtonPressed += MoveOrder;
    }
    private void OnDisable() {
        PlayerInput.Instance.onLeftMouseButtonPressed -= MoveOrder;
    }


    private void OnStartPlayerTurn(int entityID)
    {
        //Check event to see if this id matches
        // if(entity.GetInstanceID() == GetInstanceID())
        if (entityID == gameObject.GetInstanceID())

        {
            //canMove = true;
            //Debug.Log(gameObject.name + " Start turn");
            UpdateMaxDistance();
        }

    }
    private void OnFinishPlayerTurn(int entityID)
    {
        //Check event to see if this id matches
        if (entityID == GetInstanceID())
        {
            //canMove = false;
            UpdateMaxDistance();
        }

    }
    public void UpdateMaxDistance()
    {
        //Update according to remaining actions
        if (turnScheduler && turnScheduler.actionsRemaining > 1)
        {
            maxDistanceCurrent = maxDistance2;
            distanceChecker2.SetActive(true);

        }
        else
        {
            maxDistanceCurrent = maxDistance1;
            distanceChecker2.SetActive(false);

        }
       // Debug.Log("MaxDist " + maxDistanceCurrent);
    }

    Vector2 AlignToGrid(Vector2 input) {
        return new Vector2(input.x.RoundToValue(0.5f), input.y.RoundToValue(0.5f));
    }
    ///// <summary>
    /////
    ///// </summary>
    ///// <param name="input">Number to be rounded</param>
    ///// <returns>Float rounded to nearest x.5 value </returns>
    //float RoundToPoint5(float input) {
    //    float output = input;
    //    output -= 0.5f;
    //    output = Mathf.Round(output);
    //    output += 0.5f;

    //    return output;
    //}

    public void MoveOrder(Vector2 worldPoint2d)
    {
        //If already moving, don't bother.
        //For AI testing
        //if (seeking) return;

        //Debug.Log("Move order issued");

        bool validMove = false;

        //RaycastHit2D hit = Physics2D.Raycast(worldPoint2d, Vector2.zero);

        ////Distinguish which distance was used
        //if (hit.collider != null)
        //{
        //    Debug.Log($"Hit a collider! Its name is {hit.collider.gameObject.name}");
        //    if(hit.collider.gameObject.name == distanceChecker1.name)
        //    {
        //    //  Debug.Log("Distance1");
        //        turnScheduler.SpendActions(1);
        //        validMove = true;


        //    }
        //    else if (hit.collider.gameObject.name == distanceChecker2.name)
        //    {
        //    //    Debug.Log("Distance2");
        //        turnScheduler.SpendActions(2);
        //        validMove = true;
        //    }
        //} else {
        ////    Debug.Log("Move order hit no collider.");
        //}

        Vector2 pos2d = new Vector2(transform.position.x, transform.position.y);
        float distance = (pos2d - worldPoint2d).magnitude;

        if (distance < maxDistanceForOneAction * myEntity.TurnScheduler.actionsRemaining) {
            //Use division to find number of actions spent
            int actionsToSpend = Mathf.CeilToInt(distance / maxDistanceForOneAction);

            myEntity.TurnScheduler.SpendActions(actionsToSpend);
            validMove = true;
        }

        if (!validMove) {
        //    Debug.Log("Move order invalid, aborting.");
            return;
        } else {
        //    Debug.Log("Move order valid!");
            seeking = true;
        }

        //Align move position to grid
        Vector2 position = AlignToGrid(worldPoint2d);

        //Ensure the target object is instantiated
        if (!target)
        {
            target = Instantiate(moveTarget).transform;
            target.position = position;
            aiDestination.target = target;
        }
        else
        {
            target.position = position;
        }
        //  Debug.Log(target.position);
         UpdateMaxDistance();

    }

    void Update(){
        if (seeking && aiPath.reachedEndOfPath){
            seeking = false;
            // Debug.Log("Reached Destination");
        }
    }
}
