using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    //Custom Functionality
    public bool seekingActive;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }


    void ClickToMove()
    {
        var mousePos = Input.mousePosition;


        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);

        target.position = worldPoint2d;



        //  var mousePos = Input.mousePosition;
        //  mousePos.z = 0; // select distance = 10 units from the camera
        ////  Debug.Log(Camera.main.ScreenToWorldPoint(mousePos));

        //// target = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector3.zero).transform;


        //// target.position = Camera.main.ScreenToWorldPoint(mousePos);

        //target = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector3.zero).transform;

        seekingActive = true;
        Debug.Log(target.position);

    }

    void UpdatePath()
    {
        if (seeker.IsDone() && target)
        {

        //    Debug.Log(target.position);
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    //Check for click to move

    if (Input.GetMouseButtonDown(0))
    {
    ClickToMove();

    }

        if (path == null)
        {
          //  print("return1");
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
          //  print("return2");

            reachedEndOfPath = true;
           // seekingActive = false;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }


        //Actual Movement area
        if (seekingActive)
        {


            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            rb.AddForce(force);

     
        }
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

    }
}
