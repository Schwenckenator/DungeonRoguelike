using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PathManager : MonoBehaviour
{

    Seeker seeker;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

    }



    public List<Vector3> UpdatePath(Vector2 targetPos)
    {
 
        ABPath path2 = ABPath.Construct(transform.position, targetPos, null);
        AstarPath.StartPath(path2);
        path2.BlockUntilCalculated();
        List<Vector3> pathVectors = path2.vectorPath;


        return pathVectors;


    }


}

