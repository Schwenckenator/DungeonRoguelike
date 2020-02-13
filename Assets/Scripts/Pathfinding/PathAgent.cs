using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridPathfinding {
    [RequireComponent(typeof(Entity))]
    public class PathAgent : MonoBehaviour {
        public float walkSpeed;
        public int maxDistance;

        Vector2Int[] lastPath;

        Vector2Int origin;
        Vector2Int goal;

        bool isMoving = false;
        int pathIndex = 0;

        private Entity myEntity;

        public void Initialise() {
            myEntity = GetComponent<Entity>();
        }
        // Update is called once per frame
        void Update() {

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
        private void OnEnable() {
            //I have been enabled, it must be my turn!
            SetOrigin(transform.position);
        }
        void SetOrigin(Vector2 point) {

            origin = point.RoundToInt();
            goal = Vector2Int.zero;
            BreadthFirstPathfinder.Instance.SetOrigin(origin, maxDistance);
        }

        void PathFind() {
            //NodeMap.Instance.GetPath(origin, goal, out lastPath, out float distance);
            //AstarPathfinder.Instance.StartCoroutine(AstarPathfinder.Instance.GetPathAsync(origin, goal, FoundPath));
            lastPath = BreadthFirstPathfinder.Instance.GetPath(goal, out int length);
            pathIndex = 0;
            isMoving = true;
            myEntity.TurnScheduler.ActionStarted();
            if (length < maxDistance * 10 / 2) { // Half max distance
                myEntity.TurnScheduler.SpendActions(1);
            }else if(length < maxDistance * 10) {
                myEntity.TurnScheduler.SpendActions(2);
            } else {
                myEntity.TurnScheduler.SpendActions(100);
                Debug.Log("Spent 100 actions, this is dumb code.");
            }
        }

        void Walk() {
            if ((transform.position - goal.ToVector3Int()).sqrMagnitude < 0.01f) {
                //Reached the goal!
                transform.position = goal.ToVector3Int();
                isMoving = false;
                SetOrigin(goal);
                myEntity.TurnScheduler.ActionFinished();
                return;
            }
            if ((transform.position - lastPath[pathIndex].ToVector3Int()).sqrMagnitude < 0.01f) {
                //Reached a sub-goal
                pathIndex++;
            }
            transform.position = Vector3.MoveTowards(transform.position, lastPath[pathIndex].ToVector3Int(), walkSpeed * Time.deltaTime);
        }
    }
}

