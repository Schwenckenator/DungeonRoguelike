using UnityEngine;
using Pathfinding;

public class ClickToMove : MonoBehaviour
{
    public GameObject parentObject;
    public GameObject moveTarget;
    AIDestinationSetter aiDestination;
    AIPath aiPath;

    public GameObject distanceChecker1;
    public GameObject distanceChecker2;

    public float maxDistanceCurrent;//= 3;
    public float maxDistance1 = 1.5f;
    public float maxDistance2 = 3;

    private bool seeking;
    private EntityTurnScheduler turnScheduler;

    //Change this according to 

    private Transform target;

    void Start()
    {
  
        aiDestination = parentObject.GetComponent<AIDestinationSetter>();
        aiPath = parentObject.GetComponent<AIPath>();

        turnScheduler = GetComponent<EntityTurnScheduler>();
        UpdateMaxDistance();
    }

    void UpdateMaxDistance()
    {
        if (turnScheduler.actionsRemaining > 1)
        {
            maxDistanceCurrent = maxDistance2;

        }
        else
        {
            maxDistanceCurrent = maxDistance1;

        }
        Debug.Log("MaxDist " + maxDistanceCurrent);
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
                print("Distance1");

            }
            else if (hit.collider.gameObject.name == distanceChecker2.name)
            {
                print("Distance2");
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

    }

    void Update()
    {

        if (seeking && aiPath.reachedEndOfPath)
        {
            seeking = false;
            Debug.Log("Reached Destination");
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
