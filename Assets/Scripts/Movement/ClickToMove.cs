using UnityEngine;
using Pathfinding;

public class ClickToMove : MonoBehaviour
{
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



    void ClickToMoveOrder()
    {
        //set to moving and seeking
        seeking = true;


        var mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint2d, Vector2.zero);

        //Distinguish which distance was used
        if (hit.collider != null)
        {
            if(hit.collider.gameObject.name == distanceChecker1.name)
            {
                Debug.Log("Distance1");
                turnScheduler.SpendActions(1);


            }
            else if (hit.collider.gameObject.name == distanceChecker2.name)
            {
                Debug.Log("Distance2");
                turnScheduler.SpendActions(2);

            }
            else
            {
                turnScheduler.SpendActions(2);

            }


        }

        //Restrict the distance in one turn/click
        Vector2 center = transform.localPosition; 
        Vector2 position = worldPoint2d;
        float actualDistance = Vector2.Distance(center, position);

        if (actualDistance > maxDistanceCurrent)
        {
            Vector2 centerToPosition = position - center;
            centerToPosition.Normalize();
            position = center + centerToPosition * maxDistanceCurrent;
        }


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
        //Update when goal reached
        if (seeking && aiPath.reachedEndOfPath)
        {
            seeking = false;
           // Debug.Log("Reached Destination");
        }
        //Check for click to move
        if (Input.GetMouseButtonDown(0))
        {
            //Prevent multiple clicks
            if (!seeking)
            {

                ClickToMoveOrder();
            }

        }

    }
}
