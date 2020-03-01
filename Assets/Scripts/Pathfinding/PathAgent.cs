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




        public int PathCheckIntDistance(Vector2 goalToCheck)
        {
            int length = 0;
            if (origin != null)
            {
                goal = goalToCheck.RoundToInt();

                var checkPath = BreadthFirstPathfinder.Instance.GetPath(goal, out length);
                if (checkPath != null)
                {
                    return length;
                }

            }
                return 0;
        }

        /// <summary>
        ///Using both Astar and Breathfirst to find closed node to goal
        /// </summary>
        public Vector2Int GoalToReachableCoord(Vector2Int origin, Vector2Int goal)
        {

            AstarPathfinder.Instance.GetPath(origin, goal, out Vector2Int[] path, out float distance);

            if (path == null)
            {
                return new Vector2Int();

            }

            //Reverse to get the closest possible to goal appear first
            Array.Reverse(path);

            int pathDistance = 0;
            foreach (Vector2 p in path)
            {
                pathDistance = PathCheckIntDistance(p);
                //TODO check for square occupied
                bool occupied=false;


                if (pathDistance > 0 && !occupied)
                {
                    return p.RoundToVector2Int();
                }
            }

            //No path found
            return origin;
        }


        #endregion

        #region Private Methods




        void SetOrigin(Vector2 point) {
            //TODO working on this at the moment
            //Open the old origin position up
            //if(origin!=null)NodeMap.instance.SetPathable(origin.Value, true);
            ////Close the new origin position
            //NodeMap.instance.SetPathable(point.RoundToVector2Int(), false);

            origin = point.RoundToInt();
            goal = Vector2Int.zero;
            int maxSteps = stepsFor1Action * myEntity.TurnScheduler.actionsRemaining;
            BreadthFirstPathfinder.Instance.SetOrigin(origin.Value, maxSteps);
        }

        bool PathFind() {
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

