using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a pathfinder that floods the grid from a start point and finds the best paths
/// </summary>
public class BreadthFirstPathfinder : MonoBehaviour
{
    public static BreadthFirstPathfinder Instance { get; private set; }
    public int maxDistance;
    private int maxScore;
    private int halfMax;
    public int size = 100;

    private bool originSet = false;
    private int[,] scoreMap;

    List<PathNode> frontier;
    List<PathNode> visited;

    PathNode currentNode;

    private void Awake() {
        Instance = this;
        scoreMap = new int[size, size];
        frontier = new List<PathNode>();
        visited = new List<PathNode>();

        //for(int x = 0; x < size; x++) {
        //    for(int y = 0; y < size; y++) {
        //        scoreMap[x, y] = 10000;
        //    }
        //}
        maxScore = maxDistance * 10;
        halfMax = maxScore / 2;

    }
    

    public void SetOrigin(Vector2Int origin) {
        StartCoroutine(SetOriginAsync(origin));
    }

    public IEnumerator SetOriginAsync(Vector2Int origin) {
        Debug.Log("Flood fill pathing started.");
        originSet = true;

        MapNode[,] map = NodeMap.GetMap();
        scoreMap = new int[size, size];
        frontier.Clear();
        visited.Clear();
        frontier.Add(new PathNode(null, origin));
        

        while(frontier.Count > 0) {
            yield return null;
            currentNode = frontier[0];
            Debug.Log($"Current node is {currentNode}.");
            frontier.Remove(currentNode);
            visited.Add(currentNode);

            Vector2Int[] directions = {
                Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left,
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1), new Vector2Int(-1,1),
            };

            List<PathNode> neighbours = new List<PathNode>();
            foreach (var next in directions) {
                int x = next.x + currentNode.position.x;
                int y = next.y + currentNode.position.y;

                if (!map[x, y].IsPathable) continue;

                Debug.Log($"x={next.x}, y={next.y}, x+y={next.x + next.y}, Abs(x+y)={Mathf.Abs(next.x + next.y)}.");
                int stepCost = 10;
                if(Mathf.Abs(next.x + next.y) != 1) {
                    stepCost = 15; //Diagonals cost more
                }
                Debug.Log($"New neighbour's stepcost is {stepCost}.");
                neighbours.Add(new PathNode(currentNode, new Vector2Int(x, y), stepCost));
            }
            foreach(var neighbour in neighbours) {
                if (frontier.Contains(neighbour)) {
                    Debug.Log($"Node {neighbour} already in frontier.");
                    continue;
                }
                if (visited.Contains(neighbour)) {
                    Debug.Log($"Node {neighbour} has been visited!");
                    continue;
                }

                neighbour.score = currentNode.score + map[neighbour.position.x, neighbour.position.y].Cost;
                neighbour.distance = currentNode.distance + neighbour.stepCost;
                scoreMap[neighbour.position.x, neighbour.position.y] = neighbour.score;
                if(neighbour.distance > maxScore) {
                    Debug.Log($"Node {neighbour} is over max score");
                } else {
                    frontier.Add(neighbour);
                }
                
            }

        }
        
        
        Debug.Log("Flood fill pathing complete.");
    }

    public Vector2Int[] GetPath(Vector2Int goal) {
        if(originSet == false) {
            Debug.Log("Origin not yet set. Aborting.");
            return null;
        }
        if (!visited.Exists(x => x.position == goal)) {
            Debug.Log("Goal is out of range.");
            return null;
        }

        PathNode firstNode = visited.Find(x => x.position == goal);
        PathNode currentNode = firstNode;
        List<Vector2Int> path = new List<Vector2Int>();

        while(currentNode != null) {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path.ToArray();
    }


    private void OnDrawGizmos() {
        if (originSet) {
            

            foreach (var node in visited) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);
            }
            foreach (var node in frontier) {

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(node.position.ToVector3Int(), 0.5f);
            }
            if(currentNode != null) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(currentNode.position.ToVector3Int(), 0.5f);
            }
        }
    }
}
