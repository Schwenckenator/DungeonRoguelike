using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathManager : MonoBehaviour
{

    Seeker seeker;
    Rigidbody2D rb;

    //Highlight Vars
    public GameObject highlightGroundGO;
    private Renderer highlightGroundRenderer;
    public Color pathValidColor;
    public Color pathInvalidColor;
    public bool highlightGroundActive;
    private Vector2 lastHighlightPosition;



    //From Click to move
    public List<Vector3> foundPathCoords;



    public void Initialise()
    {
        highlightGroundGO = Instantiate(highlightGroundGO);
        highlightGroundGO.transform.position = transform.position;
        highlightGroundRenderer = highlightGroundGO.GetComponent<Renderer>();
        highlightGroundRenderer.enabled = true;
        Debug.Log("Set highlight rend enabled");

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



    private void OnEnable()
    {
        Debug.Log("PATH MANAGER OnEnable");

        PlayerInput.Instance.onMouseHover += HighlightPath;

    }
    private void OnDisable()
    {
        PlayerInput.Instance.onMouseHover -= HighlightPath;

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PATH MANAGER Start");
        highlightGroundGO = Instantiate(highlightGroundGO);
        highlightGroundGO.transform.position = transform.position;

        highlightGroundRenderer = highlightGroundGO.GetComponent<Renderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

    }

    private void OnStartPlayerTurn(int entityID)
    {
        //Check event to see if this id matches
        if (entityID == gameObject.GetInstanceID())

        {
            //Quick fix to stop incorrect position being blocked off, would like to improve this later

            Debug.Log("PATH MANAGER OnEnable");

            highlightGroundActive = true;

        }



    }
    private void OnFinishPlayerTurn(int entityID)
    {
        if (entityID == GetInstanceID())
        {
            highlightGroundActive = false;
        }
    }

    public List<Vector3> GetPathCoords(Vector2 targetPos)
    {
 
        ABPath path2 = ABPath.Construct(transform.position, targetPos, null);
        AstarPath.StartPath(path2);
        path2.BlockUntilCalculated();
        List<Vector3> pathVectors = path2.vectorPath;


        return pathVectors;


    }


    //From Click To Move

    public bool CheckValidMove(Vector2 worldPoint2d)
    {
        bool validMove = true;
        float baseX = worldPoint2d.x;
        float baseY = worldPoint2d.y;
        float testX = 0;
        float testY = 0;

        //TODO make this into extension
        float offset = -.5f;
        float minCoord = 0.1f + offset;
        float maxCoord = 0.9f + offset;

        GraphNode node;
        double foundWalkable = 0;
        double countedNodes = 0;
        for (float y = minCoord; y < maxCoord; y += 0.05f)
        {
            for (float x = minCoord; x < maxCoord; x += 0.05f)
            {
                //Get the decimal nodes within a tile
                testY = baseY + y;
                testX = baseX + x;
                node = AstarPath.active.GetNearest(new Vector2(testX, testY)).node;


                if (node.Walkable == true)
                {
                    foundWalkable += 1;
                }
                countedNodes++;
            }

        }

        if (foundWalkable / countedNodes >= 0.5)
        {

            foundPathCoords= GetPathCoords(worldPoint2d);
            float pathDistance = Mathf.Round((foundPathCoords.Count - 1) / 4);
            if (pathDistance > 0)
            {
                //Debug.Log("Distance is: " + pathDistance);
            }
            validMove = true;


        }
        else
        {
            validMove = false;

        }
        //Debug.Log("Checking Walkable Square " + worldPoint2d.x + " " + worldPoint2d.y);
        //Debug.Log("foundWalkable " + (foundWalkable / countedNodes));
        //Debug.Log("foundWalkable " + foundWalkable +" / "+ countedNodes);

        return validMove;
    }


    public void HighlightPath(Vector2 worldPoint2d)
    {


        if (highlightGroundActive)
        {

            Vector2 position = AlignToGridOffset(worldPoint2d);
            if (position != lastHighlightPosition)
            {
                foundPathCoords = GetPathCoords(worldPoint2d);
                //  Debug.Log("Path coords" + drawPath.UpdatePath(worldPoint2d));

                //Debug.Log("Highlight x " + position.x+ " y " + position.y);


                if (!highlightGroundRenderer)
                    highlightGroundRenderer = highlightGroundGO.GetComponent<Renderer>();
                    highlightGroundRenderer.enabled = true;
                    


                lastHighlightPosition = position;
                bool validMove = CheckValidMove(position);

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
        }
        else
        {
            if(highlightGroundRenderer)
                highlightGroundRenderer.enabled = false;

        }

    }
    Vector2 AlignToGridOffset(Vector2 input)
    {
        return new Vector2(Mathf.Round(input.x), Mathf.Round(input.y));

    }

}

