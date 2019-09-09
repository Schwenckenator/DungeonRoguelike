using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public enum TileLayer { noCollision, collision }

[System.Serializable]
public class TileInfo {
    public Color colour;
    public TileBase tile;
    public TileLayer layer;
}

public class DungeonGenerator : MonoBehaviour
{
    public int roomsPerLevel;
    public int maxSize;
    public Tilemap floorMap;
    public Tilemap wallMap;
    
    public RoomList roomContainer;
    public List<TileInfo> tilePairs;

    private Area dungeonArea;
    private bool showFilledArea = false;

    private Vector2Int mapCentre;
    private Vector2Int previousPosition;
    private List<Vector2Int> roomCentres;
    private List<Vector2IntPair> connections;

    // Start is called before the first frame update
    void Start()
    {
        dungeonArea = new Area(maxSize);

        mapCentre = new Vector2Int(dungeonArea.size / 2, dungeonArea.size / 2);
        previousPosition = mapCentre;

        roomCentres = new List<Vector2Int>();
        connections = new List<Vector2IntPair>();
    }

    void Scan() {
        AstarPath.active.Scan();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) { //sCan
            AstarPath.active.Scan();
        }
        if (Input.GetKeyDown(KeyCode.E)) { // Room
            AddRoomsWanderer(1);
        }
        if (Input.GetKeyDown(KeyCode.P)) { //Wanderer
            AddRoomsWanderer(roomsPerLevel);
        }
        if (Input.GetKeyDown(KeyCode.R)){ //Random
            AddRoomsRandom(roomsPerLevel);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            showFilledArea = true;
            Invoke("ResetShowFilledArea", 1.5f);
        }
    }

    private void OnDrawGizmos() {
        if (!showFilledArea) return;

        bool [,] area = dungeonArea.GetArea();

        for(int x = 0; x < area.GetUpperBound(0); x++) {
            for(int y = 0; y < area.GetUpperBound(1); y++) {
                if (area[x, y]) {
                    Vector2 centre = new Vector2(x+0.5f, y + 0.5f);
                    Gizmos.DrawWireSphere(centre, 0.5f);
                }
            }
        }
    }

    void ResetShowFilledArea() {
        showFilledArea = false;
    }

    void AddRoomsWanderer(int rooms) {
        
        GenerateLevelWanderer(rooms);

        Invoke("Scan", 0.2f);
    }
    void AddRoomsRandom(int rooms) {

        GenerateLevelRandom(rooms);
        GenerateHallways(roomCentres);

        Invoke("Scan", 0.2f);
    }
    /// <summary>
    /// Generates an entire level of map
    /// </summary>
    void GenerateLevelWanderer(int numberOfRooms) {
        Vector2Int offset = Vector2Int.zero;
        
        for (int i=0; i<numberOfRooms; i++) {
            
            int roomID = Random.Range(0, roomContainer.rooms.Length);
            //Remember its place origin
            Vector2Int index = previousPosition;

            bool placeFound = false;
            int infiniteLoopProtector = 1000;

            int scanDirection = Random.Range(0, 4); //Right, Up, Left, Down

            while (!placeFound) {

                //CheckCornersEmpty(index, roomContainer.rooms[roomID].width, roomContainer.rooms[roomID].height)
                if (!dungeonArea.IsFilled(
                    index.x, 
                    index.y, 
                    index.x + roomContainer.rooms[roomID].width - 1, 
                    index.y + roomContainer.rooms[roomID].height - 1))
                {
                    offset = index;
                    previousPosition = index;
                    placeFound = true;

                } else {
                    //Scan in direction
                    if (scanDirection == 0) { // Right
                        index.x++;
                    } else if (scanDirection == 1) { // Up
                        index[1]++;
                    } else if (scanDirection == 2) { // Left
                        index[0]--;
                    } else if (scanDirection == 3) { // Down
                        index[1]--;
                    }
                    
                    //If the generator has wandered out of bounds
                    if (index.x >= dungeonArea.size || index.x < 0 || index.y >= dungeonArea.size || index.y < 0) {
                        Debug.Log("Wandered out of bounds.");
                        break;
                    }
                }

                //Protect against infinite loops
                if (infiniteLoopProtector-- <= 0) break;
            }
            if (placeFound) {
                GenerateRoom(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);
            } else {
                Debug.Log("Failed to generate room.");
            }
            
            //GenerateRoom(roomContainer.rooms[roomID], TileLayer.noCollision, offset, floorMap, tilePairs);
        }
        
    }

    void GenerateLevelRandom(int numberOfRooms){
        int generatedRooms = 0;
        Vector2Int offset = Vector2Int.zero;

        for (int i = 0; i < numberOfRooms; i++) {

            int roomID = Random.Range(0, roomContainer.rooms.Length);
            Vector2Int roomSize = new Vector2Int (roomContainer.rooms[roomID].width, roomContainer.rooms[roomID].height);

            bool placeFound = false;
            int infiniteLoopProtector = 100;

            while (!placeFound) {
                Vector2Int randomPos = new Vector2Int
                    (Random.Range(0, dungeonArea.size - roomSize.x),
                    Random.Range(0, dungeonArea.size - roomSize.y));

                if (!dungeonArea.IsFilled(
                    randomPos.x,
                    randomPos.y,
                    randomPos.x + roomSize.x - 1,
                    randomPos.y + roomSize.y - 1)) {

                    offset = randomPos;
                    placeFound = true;
                    generatedRooms++;

                    Vector2Int centre = offset + new Vector2Int(roomSize.x / 2, roomSize.y / 2);
                    roomCentres.Add(centre);
                }

                //Protect against infinite loops
                if (infiniteLoopProtector-- <= 0) break;
            }
            if (placeFound) {
                GenerateRoom(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);
            } else {
                Debug.Log("Failed to generate room.");
            }

            //GenerateRoom(roomContainer.rooms[roomID], TileLayer.noCollision, offset, floorMap, tilePairs);
        }
        Debug.Log($"Dungeon Generated Randomly! {generatedRooms} rooms successfully generated.");
    }
    
    void GenerateHallways(List<Vector2Int> roomCentres) {
        //  For each centre
        //      find closest room that isn't already connected
        //      Connect to that room
        
        //  For each centre
        foreach (var centre in roomCentres) {
            Vector2Int closestNeighbour = new Vector2Int (10000, 10000); // Arbitrarily large number
            Vector2IntPair newConnection = null;

            Debug.Log($"I'm a room! My position is {centre}.");

            //find closest room that isn't already connected
            foreach (var neighbour in roomCentres) {
                
                // Don't count itself
                if (centre == neighbour) continue;

                Debug.Log($"My current closest neighbour's position is {closestNeighbour}.\n Checking neighbour {neighbour}...");
                if ((neighbour - centre).sqrMagnitude < (closestNeighbour - centre).sqrMagnitude) {
                    Debug.Log("It's closer!");
                    Debug.Log("Check for similar connection...");

                    bool connectionExists = false;
                    Vector2IntPair tempConnection = new Vector2IntPair(centre, neighbour);
                    foreach(var connection in connections) {
                        Debug.Log($"Does {tempConnection} equal {connection}?");
                        if (tempConnection.Equals(connection)) {
                            Debug.Log("Similar connection exists. Skipping.");
                            connectionExists = true;
                        }
                    }

                    if (!connectionExists) {
                        Debug.Log("New closest neighbour found!");
                        closestNeighbour = neighbour;
                        newConnection = tempConnection;
                    }
                }
            }
            if (newConnection != null) {
                Debug.Log($"Added Connection {newConnection.ToString()}.");
                connections.Add(newConnection);
            }
            Vector2 drawCentre = centre;
            Vector2 drawNeighbour = closestNeighbour;
            //Connect to that room
            Debug.DrawLine(drawCentre, drawNeighbour, Color.cyan, 10f);

        }
    }
    /// <summary>
    /// Generates a single room
    /// </summary>
    void GenerateRoom(Texture2D image, TileLayer layer, Vector2Int offset, Tilemap tileMap, List<TileInfo> tiles) {

        for (int x = 0; x < image.width; x++) {
            for (int y = 0; y < image.height; y++) {

                Color currentColour = image.GetPixel(x, y);
                TileInfo tileInfo = null;

                foreach (var tile in tiles) {
                    if(currentColour == tile.colour) {

                        tileInfo = tile;
                        break;
                    }
                }
                if(tileInfo == null) {
                    Debug.LogError("TileInfo is null!");
                }
                if(layer == tileInfo.layer) {
                    tileMap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), tileInfo.tile);
                }
            }
        }

        dungeonArea.SetFilled(true, offset.x, offset.y, offset.x + image.width, offset.y + image.height);
    }


    //bool CheckCornersEmpty(Vector2Int origin, int width, int height) {
    //    Vector2 drawOrigin = origin;
    //    //Draw checking box
    //    Debug.DrawLine(drawOrigin, new Vector2(origin.x, origin.y + height), Color.red, 3f);
    //    Debug.DrawLine(drawOrigin, new Vector2(origin.x + width, origin.y), Color.red, 3f);
    //    Debug.DrawLine(new Vector2(origin.x + width, origin.y), new Vector2(origin.x + width, origin.y + height), Color.red, 3f);
    //    Debug.DrawLine(new Vector2(origin.x, origin.y + height), new Vector2(origin.x + width, origin.y + height), Color.red, 3f);

    //    //bottom left
    //    if (dungeonArea.IsFilled(origin.x, origin.y)) return false;
    //    //bottom right
    //    if (dungeonArea.IsFilled(origin.x+width-1, origin.y)) return false;
    //    //top left
    //    if (dungeonArea.IsFilled(origin.x, origin.y+height-1)) return false;
    //    //top right
    //    if (dungeonArea.IsFilled(origin.x+width-1, origin.y+height-1)) return false;

        
    //    //If it gets here, it's all free!
    //    return true;
    //}
}
