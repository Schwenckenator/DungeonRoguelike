using UnityEngine;
using Pathfinding;

public class ClickToMove : MonoBehaviour
{
    private Transform target;
    public GameObject moveTarget;
    AIDestinationSetter aiDestination;

    public GameObject distanceChecker1;
    public GameObject distanceChecker2;
    public float maxDistance=3;


    //Change this according to 
    public bool selected = true;

    void Start()
    {

        aiDestination = GetComponent<AIDestinationSetter>();
    }


    void ClickToMoveOrder()
    {
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

        if (actualDistance > maxDistance)
        {
            Vector2 centerToPosition = position - center;
            centerToPosition.Normalize();
            position = center + centerToPosition * maxDistance;
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
        //Check for click to move
        if (Input.GetMouseButtonDown(0))
        {
            ClickToMoveOrder();

        }
    }
}
