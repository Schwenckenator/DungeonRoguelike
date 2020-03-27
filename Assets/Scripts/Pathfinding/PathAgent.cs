using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace GridPathfinding {
    [RequireComponent(typeof(Entity))]
    public class PathAgent : MonoBehaviour {
        public float walkSpeed;
        public int stepsFor1Action;

        Vector2Int[] lastPath;

        Vector2Int? origin;
        Vector2Int goal;

        bool isMoving = false;
        int pathIndex = 0;

        private Entity myEntity;

        public void Initialise() {
            myEntity = GetComponent<Entity>();

            //Reserve my current space
            NodeMap.SetOccupied(transform.position.RoundToInt().ToVector2Int(), true);
        }

        #region Unity Callbacks
        private void Update() {
            if (Input.GetKeyDown(KeyCode.L)) {
                //PathBoundaryManager.SetLines(stepsFor1Action, stepsFor1Action * myEntity.TurnScheduler.actionsRemaining);
            }
        }


        private void FixedUpdate() {
            if (isMoving) {
                Walk();
            }
        }

        private void OnEnable() {
            //I have been enabled, it must be my turn!
            SetOrigin(transform.position);
            if(myEntity.allegiance == EntityAllegiance.hero) {
                PlayerInput.Instance.onRightMouseButtonPressed += SetGoalAndFindPath;
            }
        }
        private void OnDisable() {
            if (myEntity.allegiance == EntityAllegiance.hero) {
                PlayerInput.Instance.onRightMouseButtonPressed -= SetGoalAndFindPath;
            }
        }
        #endregion

        public void SetGoalAndFindPath(Vector2 point) {
            if (origin != null) {
                goal = point.RoundToInt();
                PathFind();
            }
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
                return origin;

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

        public void FreeMySpace() {
            NodeMap.SetOccupied(transform.position.RoundToVector2Int(), false);
        }

        #region Private Methods
        void SetOrigin(Vector2 point) {
            origin = point.RoundToInt();
            goal = Vector2Int.zero;
            int maxSteps = stepsFor1Action * myEntity.TurnScheduler.actionsRemaining;
            BreadthFirstPathfinder.Instance.SetOrigin(origin.Value, maxSteps, stepsFor1Action);
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

            //Debug.Log($"Length is {length}. Divided by {BreadthFirstPathfinder.StepsToDistance(stepsFor1Action)}. ActionCost is {actionCost}");

            myEntity.TurnScheduler.ActionStarted();
            myEntity.TurnScheduler.SpendActions(actionCost);
            return true;
        }

        void Walk() {
            if ((transform.position - goal.ToVector3Int()).sqrMagnitude < 0.01f) {
                //Reached the goal!
                transform.position = goal.ToVector3Int();

                //Free original space, restrict new space
                NodeMap.SetOccupied(origin.Value, false);
                NodeMap.SetOccupied(goal, true);

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

