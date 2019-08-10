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
    public float maxDistance1 = 1.5f;
    public float maxDistance2 = 3;

    private bool seeking;
    private EntityTurnScheduler turnScheduler;
    private Transform target;

    void Start()
    {
  
        aiDestination = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        turnScheduler = GetComponent<EntityTurnScheduler>();

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


    private void OnStartPlayerTurn(int entityID)
    {
        //Check event to see if this id matches
        // if(entity.GetInstanceID() == GetInstanceID())
        if (entityID == gameObject.GetInstanceID())

        {
            //canMove = true;
            Debug.Log(gameObject.name + " Start turn");
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
        return new Vector2(RoundToPoint5(input.x), RoundToPoint5(input.y)); ;
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="input">Number to be rounded</param>
    /// <returns>Float rounded to nearest x.5 value </returns>
    float RoundToPoint5(float input) {
        float output = input;
        output -= 0.5f;
        output = Mathf.Round(output);
        output += 0.5f;

        return output;
    }

    void ClickToMoveOrder()
    {
        //Debug.Log("Move order issued");
        //set to moving and seeking
        //seeking = true;  <---- This is set later
        bool validMove = false;


        var mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint2d, Vector2.zero);

        //Distinguish which distance was used
        if (hit.collider != null)
        {
            Debug.Log($"Hit a collider! Its name is {hit.collider.gameObject.name}");
            if(hit.collider.gameObject.name == distanceChecker1.name)
            {
            //  Debug.Log("Distance1");
                turnScheduler.SpendActions(1);
                validMove = true;


            }
            else if (hit.collider.gameObject.name == distanceChecker2.name)
            {
            //    Debug.Log("Distance2");
                turnScheduler.SpendActions(2);
                validMove = true;
            }
        } else {
        //    Debug.Log("Move order hit no collider.");
        }

        if (!validMove) {
        //    Debug.Log("Move order invalid, aborting.");
            return;
        } else {
        //    Debug.Log("Move order valid!");
            seeking = true;
        }

        ////Restrict the distance in one turn/click
        //Vector2 center = transform.localPosition; 
        //Vector2 position = worldPoint2d;
        //float actualDistance = Vector2.Distance(center, position);

        //if (actualDistance > maxDistanceCurrent)
        //{
        //    Vector2 centerToPosition = position - center;
        //    centerToPosition.Normalize();
        //    position = center + centerToPosition * maxDistanceCurrent;
        //}

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

    void Update()
    {
        //if (canMove)
        //{
            //Update when goal reached
            if (seeking && aiPath.reachedEndOfPath)
            {
                seeking = false;
                // Debug.Log("Reached Destination");
            }
            //Check for click to move
            if (Input.GetMouseButtonDown(0))
            {
                // If the pointer is over a UI element, the player doesn't want to move their unit.
                if (EventSystem.current.IsPointerOverGameObject()) return;

                //Prevent multiple clicks
                if (!seeking)
                {
                    ClickToMoveOrder();
                }
            }
        //}
    }
}
