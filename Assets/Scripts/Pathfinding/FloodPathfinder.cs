using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a pathfinder that floods the grid from a start point and finds the best paths
/// </summary>
public class FloodPathfinder : MonoBehaviour
{
    public static FloodPathfinder Instance { get; private set; }
    public int maxDistance;
    private int maxScore;
    private int halfMax;
    public int size = 100;

    private Vector2Int origin;
    private Vector2Int goal;

    private bool originSet = false;
    private int[,] scoreMap;

    List<FloodPathNode> frontier;
    List<FloodPathNode> visited;

    FloodPathNode currentNode;

    private void Awake() {
        Instance = this;
        scoreMap = new int[size, size];
        frontier = new List<FloodPathNode>();
        visited = new List<FloodPathNode>();

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

        frontier.Add(new FloodPathNode(null, origin));
        

        while(frontier.Count > 0) {
            yield return null;
            currentNode = frontier[0];
            Debug.Log($"Current node is {currentNode}.");
            frontier.Remove(currentNode);

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            List<FloodPathNode> neighbours = new List<FloodPathNode>();
            foreach (var next in directions) {
                int x = next.x + currentNode.position.x;
                int y = next.y + currentNode.position.y;

                if (!map[x, y].IsPathable) continue;
                neighbours.Add(new FloodPathNode(currentNode, new Vector2Int(x, y)));
            }
            foreach(var neighbour in neighbours) {
                if (visited.Contains(neighbour)) continue;

                neighbour.score = currentNode.score + map[neighbour.position.x, neighbour.position.y].Cost;
            }

        }
        
        
        Debug.Log("Flood fill pathing complete.");
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
            //if(currentNode != null) {
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawWireSphere(currentNode.position.ToVector3Int(), 0.5f);
            //}
        }
    }
}
