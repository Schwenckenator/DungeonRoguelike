using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace GridPathfinding {
    /// <summary>
    /// This connects a GameObject to the Pathfinding System, and allows them to find paths.
    /// </summary>
    [RequireComponent(typeof(Entity))]
    public class PathAgent : MonoBehaviour {
        
        #region Public Fields

        public float walkSpeed;
        public int stepsFor1Action;

        #endregion

        #region Private Fields
        Vector2Int[] lastPath;

        Vector2Int? origin;
        Vector2Int goal;

        bool isMoving = false;
        int pathIndex = 0;

        private Entity myEntity;

        #endregion

        #region Unity Callbacks
        // Update is called once per frame
        void Update() {

            if (Input.GetMouseButtonDown(1)) {
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SetGoalAndFindPath(point);
            }
            if (isMoving) {
                Walk();
            }
        }
        private void OnEnable() {
            //I have been enabled, it must be my turn!
            SetOrigin(transform.position);
        }
        #endregion

        #region Public Methods

        public void Initialise() {
            myEntity = GetComponent<Entity>();
        }

        public bool SetGoalAndFindPath(Vector2 point) {
            if (origin != null) {
                goal = point.RoundToInt();
               return PathFind();
            }
            return false;
        }




        public bool PathCheck(Vector2 start, Vector2 finish)
        {
            if (origin != null)
            {
                goal = finish.RoundToInt();

                var checkPath = BreadthFirstPathfinder.Instance.GetPath(goal, out int length);
                if (checkPath != null)
                {
                    return true;
                }

            }
                return false;
        }

        public Vector2 GoalToReachableCoord(Vector2Int origin, Vector2Int goal)
        {
            //Action<Vector2Int> aStarPath = new Action<Vector2Int> ;

            //Action<Vector2Int[]> pathCallBack = null;
            Vector2Int[] path = AstarPathfinder.Instance.GetPath(origin, goal);

            //public IEnumerator GetPathAsync(Vector2Int origin, Vector2Int goal, Action<Vector2Int[]> callback)


            return new Vector2();
        }


        #endregion

        #region Private Methods




        void SetOrigin(Vector2 point) {

            origin = point.RoundToInt();
            goal = Vector2Int.zero;
            int maxSteps = stepsFor1Action * myEntity.TurnScheduler.actionsRemaining;
            BreadthFirstPathfinder.Instance.SetOrigin(origin.Value, maxSteps);
        }

        bool PathFind() {
            //NodeMap.Instance.GetPath(origin, goal, out lastPath, out float distance);
            //AstarPathfinder.Instance.StartCoroutine(AstarPathfinder.Instance.GetPathAsync(origin, goal, FoundPath));
            lastPath = BreadthFirstPathfinder.Instance.GetPath(goal, out int length);
            if(lastPath == null) {
                Debug.Log("Path not found!");
                return false;
            }
            pathIndex = 0;
            isMoving = true;

            // ActionCost is: length, remove possible diagonal penalty, float divide by distance per action, and round up
            int actionCost = Mathf.CeilToInt((float)(length - 5) / BreadthFirstPathfinder.StepsToDistance(stepsFor1Action));

            Debug.Log($"Length is {length}. Divided by {BreadthFirstPathfinder.StepsToDistance(stepsFor1Action)}. ActionCost is {actionCost}");

            myEntity.TurnScheduler.ActionStarted();
            myEntity.TurnScheduler.SpendActions(actionCost);
            return true;
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

            //Checked lastPath exists to stop null exception errors
            if ((lastPath!=null && pathIndex < lastPath.Length) && (transform.position - lastPath[pathIndex].ToVector3Int()).sqrMagnitude < 0.01f) {
                //Reached a sub-goal
                pathIndex++;

            }
            if (lastPath != null && pathIndex < lastPath.Length) {
                transform.position = Vector3.MoveTowards(transform.position, lastPath[pathIndex].ToVector3Int(), walkSpeed * Time.deltaTime);
            }


            return;

        }
        #endregion
    }
}

