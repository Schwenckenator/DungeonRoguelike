using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridPathfinding {
    public class AstarPathfinder : MonoBehaviour {
        public static AstarPathfinder Instance { get; private set; }

        bool startedPathfinding = false;
        private List<PathNode> open;
        private List<PathNode> closed;

        private void Awake() {
            Instance = this;
        }

        public Vector2Int[] GetPath(Vector2Int origin, Vector2Int goal, out Vector2Int[] path, out float distance) {
            MapNode[,] map = NodeMap.GetMap();
            Debug.Log($"Path wanted from {origin} to {goal}!");

            distance = -1;
            path = null;

            //Initialise open and closed lists
            var open = new List<PathNode>();
            var closed = new List<PathNode>();

            //Add the start node
            open.Add(new PathNode(null, origin));

            //While the open list is not empty
            while (open.Count > 0) {
                //Debug.Log($"{open.Count} members in the open list.");
                //Grab the node with the least F value
                PathNode currentNode = open[0]; //Sorted list by F value
                                                //Debug.Log($"Current Node is {currentNode.ToString()}.");
                                                //Remove that node from the open list
                open.Remove(currentNode);
                //Add it to the closed list
                closed.Add(currentNode);

                //If current node is the goal
                if (currentNode.position == goal) {
                    //Debug.Log("Found the goal!");
                    //Grats, backtrack through parents to find path
                    List<Vector2Int> pathList = new List<Vector2Int>();
                    PathNode current = currentNode;
                    while (current != null) {
                        pathList.Add(current.position);
                        current = current.parent;
                    }
                    pathList.Reverse();
                    path = pathList.ToArray();

                    break;
                }

                //Debug.Log("Not at the goal, generating children.");
                //Generate Children
                List<PathNode> children = new List<PathNode>();
                //The children are all of the adjacent nodes
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        if (x == 0 && y == 0) continue;
                        //Debug.Log($"Checking square {x}, {y}.");
                        if (!map[x + currentNode.position.x, y + currentNode.position.y].IsPathable) continue;
                        children.Add(new PathNode(currentNode, new Vector2Int(x + currentNode.position.x, y + currentNode.position.y)));
                    }
                }

                //For each child in children
                foreach (PathNode child in children) {
                    //Debug.Log($"Checking child {child.ToString()}.");
                    //If child is on closed list
                    if (closed.Contains(child)) {
                        //Debug.Log("This child is on the closed list, ignoring.");
                        continue;
                    }
                    //Create F, G, and H values
                    //Check for diagonals
                    int diagonalPenalty = 0;
                    Vector2Int difference = child.position - currentNode.position;
                    difference.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));
                    //It is clamped to (-1,-1) and (1,1). If the magnitude is greater than 1, it means it is diagonal
                    if (difference.magnitude > 1f) {
                        diagonalPenalty = 10;
                    }

                    //Child.g = currentNode.g + distance between child and current
                    child.g = currentNode.g + map[child.position.x, child.position.y].Cost + diagonalPenalty;
                    //Child.h = distance from child to end
                    child.h = (child.position * 10 - goal * 10).sqrMagnitude;
                    //Child.f = child.g + child.h
                    child.f = child.g + child.h;
                    //Debug.Log($"Child properties: g={child.g}, h={child.h}, f={child.f}.");

                    //if child.position is in the openList's positions
                    bool bestNode = true;
                    foreach (var node in open) {
                        if (node == child) {
                            //Debug.Log("Found another node with the same position.");
                            //if child.g is greater than openlist's node.g
                            if (child.g > node.g) {
                                //Debug.Log("The other node is closer. Ignoring.");
                                bestNode = false;
                                break;
                            }

                        }
                    }
                    if (!bestNode) {
                        continue;
                    }


                    //Add child to openlist in F order
                    //Debug.Log("Finding F ordered position to add node.");

                    for (int i = 0; i < open.Count + 1; i++) {
                        if (i >= open.Count) {
                            //Debug.Log("Adding to open list at end.");
                            open.Add(child);
                            break;
                        }
                        if (child.f < open[i].f) {
                            open.Insert(i, child);
                            //Debug.Log($"Inserted child to open list at position {i}.");
                            break;
                        }
                    }
                }
            }

            if (path != null) {
                Debug.Log($"Path found with size {path.Length}");
            } else {
                Debug.Log("Path not found.");
            }


            return path;
        }



        public IEnumerator GetPathAsync(Vector2Int origin, Vector2Int goal, Action<Vector2Int[]> callback) {
            startedPathfinding = true;
            MapNode[,] map = NodeMap.GetMap();
            Debug.Log($"Path wanted from {origin} to {goal}!");
            Vector2Int[] path = null;

            //Initialise open and closed lists
            open.Clear();
            closed.Clear();

            //Add the start node
            open.Add(new PathNode(null, origin));

            //While the open list is not empty
            while (open.Count > 0) {
                yield return null;
                //Debug.Log($"{open.Count} members in the open list.");
                //Grab the node with the least F value
                PathNode currentNode = open[0]; //Sorted list by F value
                                                //Debug.Log($"Current Node is {currentNode.ToString()}.");

                //Remove that node from the open list
                open.Remove(currentNode);
                //Add it to the closed list
                closed.Add(currentNode);

                //If current node is the goal
                if (currentNode.position == goal) {
                    //Debug.Log("Found the goal!");
                    //Grats, backtrack through parents to find path
                    List<Vector2Int> pathList = new List<Vector2Int>();
                    PathNode current = currentNode;
                    while (current != null) {
                        pathList.Add(current.position);
                        current = current.parent;
                    }
                    pathList.Reverse();
                    path = pathList.ToArray();

                    break;
                }

                //Debug.Log("Not at the goal, generating children.");
                //Generate Children
                List<PathNode> children = new List<PathNode>();
                //The children are all of the adjacent nodes
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        if (x == 0 && y == 0) continue;
                        //Debug.Log($"Checking square {x}, {y}.");
                        if (!map[x + currentNode.position.x, y + currentNode.position.y].IsPathable) continue;
                        children.Add(new PathNode(currentNode, new Vector2Int(x + currentNode.position.x, y + currentNode.position.y)));
                    }
                }

                //For each child in children
                foreach (PathNode child in children) {
                    //Debug.Log($"Checking child {child.ToString()}.");
                    //If child is on closed list
                    if (closed.Contains(child)) {
                        //Debug.Log("This child is on the closed list, ignoring.");
                        continue;
                    }
                    //Create F, G, and H values
                    //Check for diagonals
                    int diagonalPenalty = 0;
                    Vector2Int difference = child.position - currentNode.position;
                    difference.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));
                    //It is clamped to (-1,-1) and (1,1). If the magnitude is greater than 1, it means it is diagonal
                    if (difference.magnitude > 1f) {
                        diagonalPenalty = 10;
                    }

                    //Child.g = currentNode.g + distance between child and current
                    child.g = currentNode.g + map[child.position.x, child.position.y].Cost + diagonalPenalty;
                    //Child.h = distance from child to end
                    child.h = (child.position * 10 - goal * 10).sqrMagnitude;
                    //Child.f = child.g + child.h
                    child.f = child.g + child.h;
                    //Debug.Log($"Child properties: g={child.g}, h={child.h}, f={child.f}.");

                    //if child.position is in the openList's positions
                    bool bestNode = true;
                    foreach (var node in open) {
                        if (node == child) {
                            //Debug.Log("Found another node with the same position.");
                            //if child.g is greater than openlist's node.g
                            if (child.g > node.g) {
                                //Debug.Log("The other node is closer. Ignoring.");
                                bestNode = false;
                                break;
                            }

                        }
                    }
                    if (!bestNode) {
                        continue;
                    }


                    //Add child to openlist in F order
                    //Debug.Log("Finding F ordered position to add node.");

                    for (int i = 0; i < open.Count + 1; i++) {
                        if (i >= open.Count) {
                            //Debug.Log("Adding to open list at end.");
                            open.Add(child);
                            break;
                        }
                        if (child.f < open[i].f) {
                            open.Insert(i, child);
                            //Debug.Log($"Inserted child to open list at position {i}.");
                            break;
                        }
                    }
                }
            }

            if (path != null) {
                Debug.Log($"Path found with size {path.Length}");
            } else {
                Debug.Log("Path not found.");
            }

            //Give the path
            callback(path);
        }

        private void OnDrawGizmos() {
            if (startedPathfinding && open!=null) {
                foreach (var node in open) {

                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);
                }
                foreach (var node in closed) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);
                }
            }
        }
    }
}
