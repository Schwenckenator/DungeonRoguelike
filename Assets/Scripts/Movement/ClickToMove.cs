﻿using UnityEngine;
using Pathfinding;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class ClickToMove : MonoBehaviour
{

    public List<Vector3> foundPathCoords;

    public GameObject moveTarget;
    AIDestinationSetter aiDestination;
    AIPath aiPath;

    public GameObject distanceChecker1;
    public GameObject distanceChecker2;

    public float finishTurnDelay = 2;
    private float maxDistanceCurrent;

    //Would like to retrieve this programatically but for short term this works
    public float maxDistance1 = 3f;
    public float maxDistance2 = 6f;
    public float maxDistanceForOneAction = 3f;

    private bool seeking;
    private EntityTurnScheduler turnScheduler;
    private Entity myEntity;
    private Transform target;

    public PathManager pathManager;


    public void Initialise()
    {
  
        aiDestination = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        turnScheduler = GetComponent<EntityTurnScheduler>();
        myEntity = GetComponent<Entity>();
        pathManager = GetComponent<PathManager>();
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
        if (entityID == gameObject.GetInstanceID())

        {
            Debug.Log(gameObject.name + " Start turn");
            UpdateMaxDistance();
            //Quick fix to stop incorrect position being blocked off, would like to improve this later
            StartCoroutine(DelayedCheckTurn(entityID));
            pathManager.highlightGroundActive = true;

        }


    }
    private void OnFinishPlayerTurn(int entityID)
    {
        //Check event to see if this id matches
        if (entityID == GetInstanceID())
        {
            UpdateMaxDistance();
            pathManager.highlightGroundActive = false;

        }

    }
    private void UpdateObstacles(int entityID)
    {
        //Any object with the Single Node Blocker will count as an obstacle
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Entity");

        foreach (GameObject go in gos)
        {
            //Check we dont add itself to the list
            if (go.GetInstanceID() == entityID)
            { //gameObject refers to the gameObject this script is attached to.
                go.layer = 0;
            }
            else
            {
                go.layer = 8;

            }
        }
        //Debug.Log("Update Obstacles called");


    }
    public void UpdateMaxDistance()
    {
        //Update according to remaining actions
        if (turnScheduler && turnScheduler.actionsRemaining > 1)
        {
            maxDistanceCurrent = maxDistance2;
            distanceChecker1.SetActive(true);

            distanceChecker2.SetActive(true);
        }
        else if (turnScheduler && turnScheduler.actionsRemaining > 0)
        {
            maxDistanceCurrent = maxDistance1;
            distanceChecker1.SetActive(true);
            distanceChecker2.SetActive(false);

        }
        else
        {
            maxDistanceCurrent = maxDistance1;
            distanceChecker1.SetActive(false);
            distanceChecker2.SetActive(false);

        }
    }

    Vector2 AlignToGrid(Vector2 input) {
        return new Vector2(input.x.RoundToValue(0.5f), input.y.RoundToValue(0.5f));
    }
    Vector2 AlignToGridOffset(Vector2 input)
    {
        return new Vector2( Mathf.Round(input.x), Mathf.Round(input.y));

    }

    private bool CheckValidMove(Vector2 worldPoint2d)
    {
        //Moved to path Manager
        return pathManager.CheckValidMove(worldPoint2d);
    }

    public void MoveOrder(Vector2 worldPoint2d)
    {
        //If already moving, don't bother.
        if (seeking) {
            Debug.Log("Already Moving! Ignoring input.");
            return;
        }

        Vector2 pos2d = new Vector2(transform.position.x, transform.position.y);
        float distance = (pos2d - worldPoint2d).magnitude;

        bool validMove = CheckValidMove(worldPoint2d);
        if (validMove)
        {
            //Use division to find number of actions spent
            int actionsToSpend = Mathf.CeilToInt(distance / maxDistanceForOneAction);

            myEntity.TurnScheduler.SpendActions(actionsToSpend);
            seeking = true;
            aiPath.isStopped = false;
            aiPath.canSearch = true;
            myEntity.TurnScheduler.ActionStarted();

        }
        else{
            Debug.Log("Move order invalid, aborting.");
            return;
        }

        //Align move position to grid
        Vector2 position = AlignToGridOffset(worldPoint2d);

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

    void MoveComplete() {

            //aiPath.isStopped = true;
            //aiPath.canSearch = false;
            myEntity.TurnScheduler.ActionFinished();

    }

    IEnumerator DelayedCheckTurn(int entityID)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        UpdateObstacles(entityID);
        AstarPath.active.Scan();

    }
    private void FixedUpdate()
    {
        //Pathfinding update required new way to check destination
        if (seeking && aiPath.reachedEndOfPath)
        {
            seeking = false;
            Invoke("MoveComplete", finishTurnDelay);
        }
    }


}

