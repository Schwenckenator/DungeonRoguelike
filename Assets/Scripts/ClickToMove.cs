using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ClickToMove : MonoBehaviour
{
    private Transform target;
    public GameObject moveTarget;
    AIDestinationSetter aiDestination;

    public GameObject distanceChecker1;
    public GameObject distanceChecker2;


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

        if (hit.collider != null)
        {
            if(hit.collider.gameObject.name == distanceChecker1.name)
            {
                print("checker1 hit");

            }
            if (hit.collider.gameObject.name == distanceChecker2.name)
            {
                print("checker2 hit");

            }

        }
    
    

        if (!target)
        {
            target = Instantiate(moveTarget).transform;

            target.position = worldPoint2d;

            aiDestination.target = target;

        }
        else
        {
            target.position = worldPoint2d;
        }

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
