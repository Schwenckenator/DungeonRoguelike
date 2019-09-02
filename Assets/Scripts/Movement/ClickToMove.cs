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
    private Vector2 lastHighlightPosition;


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
        // if(entity.GetInstanceID() == GetInstanceID())
        if (entityID == gameObject.GetInstanceID())

        {
            //canMove = true;
            Debug.Log(gameObject.name + " Start turn");
            UpdateMaxDistance();
            UpdateObstacles(entityID);
            AstarPath.active.Scan();

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

    public void HighlightPath(Vector2 worldPoint2d)
    {
        Vector2 position = AlignToGrid(worldPoint2d);
        //Reduce updates on same position
        if (position != lastHighlightPosition)
            lastHighlightPosition = position;
            highlightGroundGO.transform.position = position;

            highlightGroundRenderer.material.color = pathInvalidColor;
            highlightGroundRenderer.material.color = pathValidColor;

        {
            lastHighlightPosition = position;
            highlightGroundGO.transform.position = position;


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

        bool validMove = false;

        Vector2 pos2d = new Vector2(transform.position.x, transform.position.y);
        float distance = (pos2d - worldPoint2d).magnitude;

        if (distance < maxDistanceForOneAction * myEntity.TurnScheduler.actionsRemaining) {
            //Use division to find number of actions spent
            int actionsToSpend = Mathf.CeilToInt(distance / maxDistanceForOneAction);

            myEntity.TurnScheduler.SpendActions(actionsToSpend);
            validMove = true;
        }

        if (!validMove) {
            Debug.Log("Move order invalid, aborting.");
            return;
        } else {
        //    Debug.Log("Move order valid!");
            seeking = true;
            myEntity.TurnScheduler.ActionStarted();
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
