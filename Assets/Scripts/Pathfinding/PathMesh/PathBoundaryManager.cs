using GridPathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathBoundaryManager : MonoBehaviour
{
    private static PathBoundaryManager Instance { get; set; }
    public PathBoundary[] pathBoundaries;
    
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        pathBoundaries[0].Initialise();
        pathBoundaries[1].Initialise();
    }

    public static void SetupBoundaries(PathNode[] oneMove, PathNode[] maxMove) {

        Debug.Log($"One move has {oneMove.Length} nodes.");
        Debug.Log($"Max move has {maxMove.Length} nodes.");
        
        Instance.pathBoundaries[0].Apply(oneMove);
        Instance.pathBoundaries[1].Apply(maxMove);
    }

    public static void ClearBoundaries() {
        Instance.pathBoundaries[0].HideLine();
        Instance.pathBoundaries[1].HideLine();
    }

}
