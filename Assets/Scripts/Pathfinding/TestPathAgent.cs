using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathAgent : MonoBehaviour
{

    Vector2Int[] lastPath;

    Vector2Int origin;
    Vector2Int goal;

    private void Start() {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            origin = point.RoundToInt();
            SetOrigin();
        }
        if (Input.GetMouseButtonDown(1)) {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            goal = point.RoundToInt();
            if(origin != Vector2Int.zero) {
                PathFind();
            }
        }
    }
    void SetOrigin() {
        FloodPathfinder.Instance.SetOrigin(origin);
    }

    void PathFind() {
        //NodeMap.Instance.GetPath(origin, goal, out lastPath, out float distance);
        //AstarPathfinder.Instance.StartCoroutine(AstarPathfinder.Instance.GetPathAsync(origin, goal, FoundPath));

    }

    public void FoundPath(Vector2Int[] path) {
        lastPath = path;
    }

    private void OnDrawGizmos() {
        if(origin != Vector2Int.zero) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin.ToVector3Int(), 0.5f);
        }
        if (goal != Vector2Int.zero) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(goal.ToVector3Int(), 0.5f);
        }
        if(lastPath != null) {
            Gizmos.color = Color.magenta;

            for (int i = 0; i < lastPath.Length - 1; i++) {
                Gizmos.DrawLine(lastPath[i].ToVector3Int(), lastPath[i + 1].ToVector3Int());
            }
        }
    }

}
