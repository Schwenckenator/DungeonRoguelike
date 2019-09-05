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

    //Highlight Vars
    public GameObject highlightGroundGO;
    private Renderer highlightGroundRenderer;
    public Color pathValidColor;
    public Color pathInvalidColor;
    private bool highlightGroundActive;

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

        //Highlight
        highlightGroundGO = Instantiate(highlightGroundGO);
        highlightGroundGO.transform.position = transform.position;
        highlightGroundRenderer = highlightGroundGO.GetComponent<Renderer>();
    }

    private void OnDestroy()
    {
        //Unsubscribe from game events
        GameEvents.current.onStartPlayerTurn -= OnStartPlayerTurn;
        GameEvents.current.onFinishPlayerTurn -= OnFinishPlayerTurn;
    }

    private void OnEnable() {
        PlayerInput.Instance.onLeftMouseButtonPressed += MoveOrder;
        PlayerInput.Instance.onMouseHover += HighlightPath;

    }
    private void OnDisable() {
        PlayerInput.Instance.onLeftMouseButtonPressed -= MoveOrder;
        PlayerInput.Instance.onMouseHover -= HighlightPath;

    }


    private void OnStartPlayerTurn(int entityID)
    {
        //Check event to see if this id matches
        if (entityID == gameObject.GetInstanceID())

        {
            //canMove = true;
            Debug.Log(gameObject.name + " Start turn");
            UpdateMaxDistance();
            UpdateObstacles(entityID);
            AstarPath.active.Scan();
            highlightGroundActive = true;
        }


    }
    private void OnFinishPlayerTurn(int entityID)
    {
        //Check event to see if this id matches
        if (entityID == GetInstanceID())
        {
            //canMove = false;
            UpdateMaxDistance();
            highlightGroundActive = false;

        }

    }
    private void UpdateObstacles(int entityID)
    {
        //Any object with the Single Node Blocker will count as an obstacle
     //  var otherNodes = FindObjectsOfType<ClickToMove>();//all possible objects with a given component
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

    }
    public void UpdateMaxDistance()
    {
        //Update according to remaining actions
        if (turnScheduler && turnScheduler.actionsRemaining > 1)
        {
            maxDistanceCurrent = maxDistance2;
            distanceChecker1.SetActive(true);

            distanceChecker2.SetActive(true);
            highlightGroundActive = true;
        }
        else if (turnScheduler && turnScheduler.actionsRemaining > 0)
        {
            maxDistanceCurrent = maxDistance1;
            distanceChecker1.SetActive(true);
            distanceChecker2.SetActive(false);
            highlightGroundActive = true;

        }
        else
        {
            maxDistanceCurrent = maxDistance1;
            distanceChecker1.SetActive(false);
            distanceChecker2.SetActive(false);
            highlightGroundActive = false;

        }
       // Debug.Log("MaxDist " + maxDistanceCurrent);
    }

    Vector2 AlignToGrid(Vector2 input) {
        return new Vector2(input.x.RoundToValue(0.5f), input.y.RoundToValue(0.5f));
    }
  
    private bool CheckValidMove(Vector2 worldPoint2d)
    {
        bool validMove = false;

        //Vector2 pos2d = new Vector2(transform.position.x, transform.position.y);
        //float distance = (pos2d - worldPoint2d).magnitude;

        //if (distance < maxDistanceForOneAction * myEntity.TurnScheduler.actionsRemaining)
        //{

        //    validMove = true;
        //}

        GraphNode node = AstarPath.active.GetNearest(worldPoint2d).node;
        if (node.Walkable)
        {
            validMove = true;
        }

        return validMove;
    }

    public void HighlightPath(Vector2 worldPoint2d)
    {
        if (highlightGroundActive)
        {
            highlightGroundRenderer.enabled = true;
            Vector2 position = AlignToGrid(worldPoint2d);

            bool validMove = CheckValidMove(worldPoint2d);

            highlightGroundGO.transform.position = position;



            if (validMove)
            {

                highlightGroundRenderer.material.color = pathValidColor;
            }
            else
            {
                highlightGroundRenderer.material.color = pathInvalidColor;
            }
        }
        else
        {
            highlightGroundRenderer.enabled = false;

        }

    }

    public void MoveOrder(Vector2 worldPoint2d)
    {
        //If already moving, don't bother.
        if (seeking) {
            Debug.Log("Already Moving! Ignoring input.");
            return;
        }

        //Debug.Log("Move order issued");
        Vector2 pos2d = new Vector2(transform.position.x, transform.position.y);
        float distance = (pos2d - worldPoint2d).magnitude;

        bool validMove = CheckValidMove(worldPoint2d);
        if (validMove)
        {
            //Use division to find number of actions spent

            int actionsToSpend = Mathf.CeilToInt(distance / maxDistanceForOneAction);

            myEntity.TurnScheduler.SpendActions(actionsToSpend);
            seeking = true;
            myEntity.TurnScheduler.ActionStarted();
        }
        else{
            Debug.Log("Move order invalid, aborting.");
            return;
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
        aiPath.onTargetReached += MoveComplete;
    }


    private void MoveComplete() {
        seeking = false;
        aiPath.onTargetReached -= MoveComplete;
        myEntity.TurnScheduler.ActonFinished();
    }
}
