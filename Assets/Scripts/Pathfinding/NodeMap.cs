using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GridPathfinding {
    public class NodeMap : MonoBehaviour {

        private MapNode[,] Map { get; set; }

        public static NodeMap instance;
        private bool displayGizmos = false;

        #region public methods

        public static MapNode[,] GetMap() {
            return instance.Map;
        }

        public void Initialise(Dungeon dungeon) {
            int size = dungeon.FilledArea.size;
            Map = new MapNode[size, size];

            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    Vector2Int point = new Vector2Int(x, y);
                    var hit = Physics2D.OverlapCircle(point, 0.45f, LayerMask.GetMask("Obstacle")); //Not quite a 1 unit diameter circle

                    bool pathable = false;
                    int cost = 10;

                    if (hit == null) pathable = true;

                    Map[x, y] = new MapNode(pathable, cost);
                }
            }
            Debug.Log($"Map generated, with size {Map.GetUpperBound(0)}, {Map.GetUpperBound(1)}");
        }

        public static void SetPathable(Vector2Int coords, bool isPathable) {
            Debug.Log($"Node {coords} set IsPathable to {isPathable}");
            instance.Map[coords.x, coords.y].IsPathable = isPathable;
        }

        public static void SetCost(Vector2Int coords, int cost) {
            throw new NotImplementedException();
        }

        #endregion

        private void Awake() {
            instance = this;
        }
        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.L)) {
                //DebugDump();
                displayGizmos = !displayGizmos;
            }
        }
        #region Private Methods

        private void OnDrawGizmos() {
            if (!displayGizmos) return;

            Gizmos.color = Color.red;

            for (int x = 0; x < Map.GetUpperBound(0); x++) {
                for (int y = 0; y < Map.GetUpperBound(1); y++) {
                    if (!Map[x, y].IsPathable) {
                        Vector2 centre = new Vector2(x, y);
                        Gizmos.DrawWireSphere(centre, 0.5f);
                    }
                }
            }

        }

        private void DebugDump() {
            Debug.Log("Logging all map data...");
            for (int x = 0; x < Map.GetUpperBound(0); x++) {
                for (int y = 0; y < Map.GetUpperBound(1); y++) {
                    Debug.Log($"Node {x},{y}. {Map[x, y].ToString()}");
                }
            }
            Debug.Log("Log dump complete.");
        }

        internal void SetPathable(Vector2Int? origin, bool v)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
