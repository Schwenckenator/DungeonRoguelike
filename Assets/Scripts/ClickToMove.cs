using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ClickToMove : MonoBehaviour
{
    private Transform target;
    public GameObject moveTarget;
    AIDestinationSetter aiDestination;

    void Start()
    {

        aiDestination = GetComponent<AIDestinationSetter>();
    }


    void ClickToMoveOrder()
    {
        var mousePos = Input.mousePosition;


        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);

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

    void FixedUpdate()
    {
    //Check for click to move
    if (Input.GetMouseButtonDown(0))
    {
            ClickToMoveOrder();

    }

    }
}
