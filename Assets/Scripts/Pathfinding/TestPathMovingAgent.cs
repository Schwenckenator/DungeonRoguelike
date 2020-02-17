using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridPathfinding {
    public class TestPathMovingAgent : MonoBehaviour {
        public float walkSpeed;
        public int maxDistance;

        Vector2Int[] lastPath;

        Vector2Int origin;
        Vector2Int goal;

        bool isMoving = false;
        int pathIndex = 0;

        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SetOrigin(point);
            }
            if (Input.GetMouseButtonDown(1)) {
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (origin != Vector2Int.zero) {
                    goal = point.RoundToInt();
                    PathFind();
                }
            }
            if (isMoving) {
                Walk();
            }
        }
        void SetOrigin(Vector2 point) {

            origin = point.RoundToInt();
            transform.position = point.RoundToInt().ToVector3Int();
            goal = Vector2Int.zero;
            BreadthFirstPathfinder.Instance.SetOrigin(origin, maxDistance);
        }

        void PathFind() {
            //NodeMap.Instance.GetPath(origin, goal, out lastPath, out float distance);
            //AstarPathfinder.Instance.StartCoroutine(AstarPathfinder.Instance.GetPathAsync(origin, goal, FoundPath));
            lastPath = BreadthFirstPathfinder.Instance.GetPath(goal, out int length);
            pathIndex = 0;
            isMoving = true;
        }

        void Walk() {
            if ((transform.position - goal.ToVector3Int()).sqrMagnitude < 0.01f) {
                //Reached the goal!
                transform.position = goal.ToVector3Int();
                isMoving = false;
                SetOrigin(goal);
                return;
            }
            if ((transform.position - lastPath[pathIndex].ToVector3Int()).sqrMagnitude < 0.01f) {
                //Reached a sub-goal
                pathIndex++;
            }
            transform.position = Vector3.MoveTowards(transform.position, lastPath[pathIndex].ToVector3Int(), walkSpeed * Time.deltaTime);
        }

        //private void OnDrawGizmos() {
        //    if (origin != Vector2Int.zero) {
        //        Gizmos.color = Color.yellow;
        //        Gizmos.DrawWireSphere(origin.ToVector3Int(), 0.5f);
        //    }
        //    if (goal != Vector2Int.zero) {
        //        Gizmos.color = Color.blue;
        //        Gizmos.DrawWireSphere(goal.ToVector3Int(), 0.5f);
        //    }
        //    if (lastPath != null) {
        //        Gizmos.color = Color.magenta;

        //        for (int i = 0; i < lastPath.Length - 1; i++) {
        //            Gizmos.DrawLine(lastPath[i].ToVector3Int(), lastPath[i + 1].ToVector3Int());
        //        }
        //    }
        //}
    }
}
