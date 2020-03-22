using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



namespace GridPathfinding {

    

    /// <summary>
    /// This is a pathfinder that floods the grid from a start point and finds the best paths
    /// </summary>
    /// 
    public class BreadthFirstPathfinder : MonoBehaviour
    {
        public static bool debug = false;

        public static bool readyToGetPath = true;
        public static BreadthFirstPathfinder Instance { get; private set; }

        public int size = 100;
        public static readonly int stepCost = 10;
        public float diagonalPenalty = 1.5f;

        readonly Vector2Int[] DIRECTIONS = {
                    Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
                    new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1), new Vector2Int(-1,1),
                };

        #region Private Fields
        private bool originSet = false;
        private int[,] scoreMap;

        List<PathNode> frontier;
        List<PathNode> visited;

        PathNode currentNode;

        private Thread pathThread; // Not sure what to do with this...

        #endregion

        #region Unity Callbacks
        private void Awake() {
            Instance = this;
            scoreMap = new int[size, size];
            frontier = new List<PathNode>();
            visited = new List<PathNode>();

            //maxScore = maxDistance * 10 + 5;
            //halfMax = maxScore / 2;
        }

        private void OnDrawGizmos()
        {
            if (originSet)
            {
                foreach (var node in visited) {
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);

                }
                foreach (var node in frontier) {

                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);
                }
                if (currentNode != null) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(currentNode.position.ToVector3Int(), 0.5f);
                }
            }
        }
        
        #endregion

        #region Public Methods
        public void SetOrigin(Vector2Int origin, int maxSteps) {
        
            StartCoroutine(SetOriginCoroutine(origin, maxSteps));
        }

        public Vector2 NearestNeighbour(Vector2 closest)
        {
            MapNode[,] map = NodeMap.GetMap();

            List<PathNode> neighbours = Neighbours(map);

            PathNode closestNode = neighbours[0];
            bool found = false;
            float closestDistance = float.PositiveInfinity;

            foreach (var neighbour in neighbours)
            {

                if (map[neighbour.position.x, neighbour.position.y].IsPathable)
                {

                    closestNode = neighbour;

                    //Debug.Log($"Node {neighbour} already in frontier.");
                    //continue;
                }


            }
            return closestNode.position;



        }

        public IEnumerator SetOriginCoroutine(Vector2Int origin, int maxSteps) {
            //if (debug) Debug.Log("Flood fill pathing started.");
            readyToGetPath = false;
            originSet = true;

            MapNode[,] map = NodeMap.GetMap();
            scoreMap = new int[size, size];
            frontier.Clear();
            visited.Clear();
            frontier.Add(new PathNode(null, origin));

            int maxDistance = (maxSteps * stepCost) + Mathf.RoundToInt(stepCost * (diagonalPenalty - 1)); // Pathfinder is allowed to overflow by 1 diagonal penalty
        

            while(frontier.Count > 0) {
                currentNode = frontier[0];
                yield return null;
                
                frontier.Remove(currentNode);
                visited.Add(currentNode);


                List<PathNode> neighbours = new List<PathNode>();
                foreach (var next in DIRECTIONS) {
                    int x = next.x + currentNode.position.x;
                    int y = next.y + currentNode.position.y;
                    bool isDiagonal = Mathf.Abs(next.x + next.y) != 1;


                    if (!map[x, y].IsPathable || map[x,y].IsOccupied) continue;
                    if (isDiagonal && (!map[x - next.x, y].IsPathable || !map[x, y - next.y].IsPathable)) continue;

                    int thisStepCost = stepCost;
                    if(isDiagonal) {
                        thisStepCost = Mathf.RoundToInt(thisStepCost * diagonalPenalty); //Diagonals cost more
                    }
                    //if (debug) Debug.Log($"New neighbour's stepcost is {stepCost}.");
                    neighbours.Add(new PathNode(currentNode, new Vector2Int(x, y), thisStepCost));
                }
                foreach(var neighbour in neighbours) {

                    //Don't re-add nodes that will be or have been visited
                    if (frontier.Contains(neighbour)) {
                        continue;
                    }
                    if (visited.Contains(neighbour)) {
                        continue;
                    }

                    neighbour.score = currentNode.score + map[neighbour.position.x, neighbour.position.y].Cost;
                    neighbour.distance = currentNode.distance + neighbour.stepCost;
                    scoreMap[neighbour.position.x, neighbour.position.y] = neighbour.score;
                    if(neighbour.distance > maxDistance) {
                        //if (debug) Debug.Log($"Node {neighbour} is over max score");
                    } else {
                        frontier.Add(neighbour);
                    }
                
                }
            }
        
            if (debug) Debug.Log("Flood fill pathing complete.");
            readyToGetPath = true;
        }

        public Vector2Int[] GetPath(Vector2Int goal, out int length) {
            length = 0;
            if (originSet == false) {
                Debug.Log("Origin not yet set. Aborting.");
                return null;
            }
            if(readyToGetPath == false) {
                Debug.LogWarning("Not yet ready to get path! Aborting...");
                return null;
            }
            if (!visited.Exists(x => x.position == goal)) {
                Debug.Log("Goal is out of range. Aborting...");
                return null;
            }

            PathNode firstNode = visited.Find(x => x.position == goal);
            PathNode currentNode = firstNode;
            List<Vector2Int> path = new List<Vector2Int>();
            if (debug) Debug.Log($"First Node's distance score is {firstNode.distance}");
            length = firstNode.distance;
            while (currentNode != null) {
                path.Add(currentNode.position);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path.ToArray();
        }

        public static int StepsToDistance(int stepCount) {
            return stepCount * stepCost;
        }
        #endregion

        #region Private Methods
        List<PathNode> Neighbours(MapNode[,] map) {
            List<PathNode> neighbours = new List<PathNode>();
            foreach (var next in DIRECTIONS) {
                int x = next.x + currentNode.position.x;
                int y = next.y + currentNode.position.y;

                if (!map[x, y].IsPathable) continue;

                //Debug.Log($"x={next.x}, y={next.y}, x+y={next.x + next.y}, Abs(x+y)={Mathf.Abs(next.x + next.y)}.");
                int thisStepCost = stepCost;
                if (Mathf.Abs(next.x + next.y) != 1) {
                    thisStepCost = Mathf.RoundToInt(thisStepCost * diagonalPenalty); //Diagonals cost more
                }
                //Debug.Log($"New neighbour's stepcost is {stepCost}.");
                neighbours.Add(new PathNode(currentNode, new Vector2Int(x, y), thisStepCost));
            }
            return neighbours;

        }
        #endregion
        //This go duplicated in the merge conflict
        //private void OnDrawGizmos() {
        //    if (originSet) {


        //        foreach (var node in visited) {
        //            Gizmos.color = new Color(1, 0, 0, 0.5f);
        //            Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);

        //        }
        //        foreach (var node in frontier) {

        //            Gizmos.color = Color.green;
        //            Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);
        //        }
        //        if(currentNode != null) {
        //            Gizmos.color = Color.blue;
        //            Gizmos.DrawWireSphere(currentNode.position.ToVector3Int(), 0.5f);
        //        }
        //    }
        //}
    }
}
