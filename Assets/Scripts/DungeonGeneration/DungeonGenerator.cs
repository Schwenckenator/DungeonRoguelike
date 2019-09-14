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

    public int minConnections;
    public int maxConnections;

    public int maxSteps;

    private Area dungeonArea;
    private bool showFilledArea = false;

    private Vector2Int mapCentre;
    private Vector2Int previousPosition;
    private List<Vector2Int> roomCentres;
    private List<Vector2IntPair> connections;
    private Dictionary<Vector2Int, bool> roomHasConnection;
    private Dictionary<Vector2Int, int> roomNumConnection;

    // Start is called before the first frame update
    void Start()
    {
        dungeonArea = new Area(maxSize);

        mapCentre = new Vector2Int(dungeonArea.size / 2, dungeonArea.size / 2);
        previousPosition = mapCentre;

        roomCentres = new List<Vector2Int>();
        roomHasConnection = new Dictionary<Vector2Int, bool>();
        roomNumConnection = new Dictionary<Vector2Int, int>();
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

        StartCoroutine(GenerateLevelRandom(rooms));
        //StartCoroutine(GenerateHallways(roomCentres));
        //StartCoroutine(GenerateShortestHallways(roomCentres));

        //Invoke("Scan", 0.2f);
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

    IEnumerator GenerateLevelRandom(int numberOfRooms){
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
                    roomHasConnection.Add(centre, false);
                }

                //Protect against infinite loops
                if (infiniteLoopProtector-- <= 0) break;
            }
            if (placeFound) {
                GenerateRoom(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);
            } else {
                Debug.Log("Failed to generate room.");
            }
            yield return null;
            //GenerateRoom(roomContainer.rooms[roomID], TileLayer.noCollision, offset, floorMap, tilePairs);
        }
        Debug.Log($"Dungeon Generated Randomly! {generatedRooms} rooms successfully generated.");

        //StartCoroutine(GenerateHallways(roomCentres));
        StartCoroutine(GenerateHallwaysWalking(roomCentres, maxSteps)); //TO DO REMOVE MAGIC NUMBER
    }

    IEnumerator GenerateHallways(List<Vector2Int> roomCentres) {
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

            yield return null;
        }

        Scan();
    }

    IEnumerator GenerateHallwaysShortUnconnectedFirst(List<Vector2Int> roomCentres) {
        //Find all possible connections
        //sort into shortest distance -> longest distance
        List<Vector2IntPair> roomConnections = FindAllRoomConnections(roomCentres);
        PopulateDictionary();
        //Make hallway connections in order

        //When do I stop? When every room has at least one connection NO DUMB
        //All rooms need to be connected to every other room, directly or indirectly
        int currentConnectionAllowance = 0;
        bool hasUnconnectedRooms = true;

        while (hasUnconnectedRooms) {

            foreach (var connection in roomConnections) {

                if (connections.Contains(connection)) continue; // Don't re-add itself

                if (roomNumConnection[connection.value1] > currentConnectionAllowance ||
                    roomNumConnection[connection.value2] > currentConnectionAllowance) continue;


                Vector2 drawCentre = connection.value1;
                Vector2 drawNeighbour = connection.value2;
                //Connect to that room
                Debug.DrawLine(drawCentre, drawNeighbour, Color.cyan, 10f);



                yield return null;
            }
        }
    }

    private void PopulateDictionary() {
        foreach(var room in roomCentres) {
            roomNumConnection.Add(room, 0);
        }
    }

    IEnumerator GenerateHallwaysShortestFirst(List<Vector2Int> roomCentres, int maxConnections) {
        //Find all possible connections
        //sort into shortest distance -> longest distance
        List<Vector2IntPair> roomConnections = FindAllRoomConnections(roomCentres);

        //Make hallway connections in order

        //When do I stop? When every room has at least one connection NO DUMB
        //All rooms need to be connected to every other room, directly or indirectly
        int totalConnections = 40;
        int currentConnections = 0;

        foreach (var connection in roomConnections) {
            Vector2 drawCentre = connection.value1;
            Vector2 drawNeighbour = connection.value2;
            //Connect to that room
            Debug.DrawLine(drawCentre, drawNeighbour, Color.cyan, 10f);
            currentConnections++;

            yield return null;

            if (currentConnections >= totalConnections) {
                break;
            }
        }

        throw new System.NotImplementedException();
    }

    IEnumerator GenerateMultipleHallways(List<Vector2Int> roomCentres, int numOfConnections) {
        //  For each centre
        //      find closest room that isn't already connected
        //      Connect to that room

        //  For each centre
        foreach (var centre in roomCentres) {
            Vector2Int closestNeighbour = new Vector2Int(10000, 10000); // Arbitrarily large number
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
                    foreach (var connection in connections) {
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

            yield return null;
        }

        Scan();
    }

    IEnumerator GenerateHallwaysWalking(List<Vector2Int> roomCentres, int stepMax) {
        //Pick the starting room at random
        Vector2Int currentRoom = roomCentres[0];
        bool unconnectedRoomsExist = true;
        int steps = 0;
        Color walkColour = RandomColour();

        while (unconnectedRoomsExist) {
            Debug.Log($"I'm a room! My position is {currentRoom}.");
            //Find the nearest neighbour
            Vector2Int nearestNeighbour = new Vector2Int(10000, 10000); //Arbitrarily large value
            Vector2IntPair nearestConnection = new Vector2IntPair(currentRoom, nearestNeighbour);
            foreach(var neighbour in roomCentres) {

                if (currentRoom == neighbour) continue; //Can't be your own neighbour

                Debug.Log($"My current closest neighbour's position is {nearestNeighbour}.\n Checking neighbour {neighbour}...");

                Vector2IntPair currentConnection = new Vector2IntPair(currentRoom, neighbour);
                if(currentConnection.sqrDistance < nearestConnection.sqrDistance) {
                    Debug.Log("It's closer!");
                    Debug.Log("Check for similar connection...");

                    if (!IsSimilarConnection(currentConnection, connections)) {
                        Debug.Log("New closest neighbour found!");
                        nearestNeighbour = neighbour;
                        nearestConnection = currentConnection;
                    }
                }

            }

            if (nearestConnection != null) {
                Debug.Log($"Added Connection {nearestConnection.ToString()}.");
                connections.Add(nearestConnection);
            }

            //Make path to that neighbour
            DrawDebugHallway(currentRoom, nearestNeighbour, walkColour);

            roomHasConnection[currentRoom] = true; //Current room has connection
            roomHasConnection[nearestNeighbour] = true; //Neigbour also has connection

            //If there are still unconnected rooms

            //yield return null;
            yield return new WaitForSeconds(0.25f);

            if (!roomHasConnection.ContainsValue(false)) {
                unconnectedRoomsExist = false;
            } else if(steps >= stepMax){
                //Set the neighbour as the next room
                currentRoom = roomCentres[0];
                steps = 0;
                walkColour = RandomColour();
                Debug.LogWarning("Finished a walk and pausing.");
                //Debug.Break();
            } else {
                currentRoom = nearestNeighbour;
                steps++;
            }
        }


        //Repeat


        throw new System.NotImplementedException();
    }

    IEnumerator GenerateHallwaysShortestWalking(List<Vector2Int> roomCentres, int maxConnections) {
        //Pick the starting room at random
        Vector2Int currentRoom = roomCentres[0];
        List<Vector2Int> roomCandidates = new List<Vector2Int>();

        bool unconnectedRoomsExist = true;
        
        Color walkColour = RandomColour();

        while (unconnectedRoomsExist) {
            Debug.Log($"I'm a room! My position is {currentRoom}.");
            //Find the nearest neighbour
            Vector2Int nearestNeighbour = new Vector2Int(10000, 10000); //Arbitrarily large value
            Vector2IntPair nearestConnection = new Vector2IntPair(currentRoom, nearestNeighbour);
            foreach (var neighbour in roomCentres) {

                if (currentRoom == neighbour) continue; //Can't be your own neighbour

                Debug.Log($"My current closest neighbour's position is {nearestNeighbour}.\n Checking neighbour {neighbour}...");

                Vector2IntPair currentConnection = new Vector2IntPair(currentRoom, neighbour);
                if (currentConnection.sqrDistance < nearestConnection.sqrDistance) {
                    Debug.Log("It's closer!");
                    Debug.Log("Check for similar connection...");

                    if (!IsSimilarConnection(currentConnection, connections)) {
                        Debug.Log("New closest neighbour found!");
                        nearestNeighbour = neighbour;
                        nearestConnection = currentConnection;
                    }
                }

            }

            if (nearestConnection != null) {
                Debug.Log($"Added Connection {nearestConnection.ToString()}.");
                connections.Add(nearestConnection);
            }

            //Make path to that neighbour
            DrawDebugHallway(currentRoom, nearestNeighbour, walkColour);

            roomHasConnection[currentRoom] = true; //Current room has connection
            roomHasConnection[nearestNeighbour] = true; //Neigbour also has connection

            //If there are still unconnected rooms

            yield return null;
            //yield return new WaitForSeconds(0.25f);

            if (!roomNumConnection.ContainsValue(0)) {
                unconnectedRoomsExist = false;
            } else {
                if (!roomCandidates.Contains(nearestNeighbour) && roomNumConnection[nearestNeighbour] <= maxConnections) {
                    roomCandidates.Add(nearestNeighbour);
                }
                
            }
            //    //Set the neighbour as the next room
            //    currentRoom = roomCentres[0];
            //    steps = 0;
            //    walkColour = RandomColour();
            //    Debug.LogWarning("Finished a walk and pausing.");
            //    //Debug.Break();
            //} else {
            //    currentRoom = nearestNeighbour;
            //    steps++;
            //}
        }
    }

    bool IsSimilarConnection(Vector2IntPair testConnection, List<Vector2IntPair> connectionList) {
        
        foreach (var connection in connectionList) {
            Debug.Log($"Does {testConnection} equal {connection}?");
            if (testConnection.Equals(connection)) {
                Debug.Log("Similar connection exists. Skipping.");
                return true;
            }
        }

        return false;
    }

    void DrawDebugHallway(Vector2Int value1, Vector2Int value2, Color colour) {
        Vector2 drawValue1 = value1;
        Vector2 drawValue2 = value2;
        //Connect to that room
        Debug.DrawLine(drawValue1, drawValue2, colour, 100f);
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

    List<Vector2IntPair> FindAllRoomConnections(List<Vector2Int> roomCentres) {

        var allConnections = new List<Vector2IntPair>();

        foreach (var room in roomCentres) {
            foreach (var neighbour in roomCentres) {
                if (room == neighbour) continue; // Don't allow connections to itself

                Vector2IntPair newConnection = new Vector2IntPair(room, neighbour);

                if (ListContainsEquivalent(allConnections, newConnection)) {
                    //Don't add, it already exists
                } else {
                    int index = FindInsertIndex(allConnections, newConnection);
                    allConnections.Insert(index, newConnection);
                }
            }
        }

        return allConnections;
    }

    bool ListContainsEquivalent(List<Vector2IntPair> connections, Vector2IntPair newPair) {
        foreach (var connection in connections) {
            if (connection.Equals(newPair)) {
                return true;
            }
        }
        return false;
    }

    int FindInsertIndex(List<Vector2IntPair> connections, Vector2IntPair newConnection) {
        int insertIndex = 0;
        for (int i = 0; i < connections.Count; i++) {
            if (connections[i].sqrDistance > newConnection.sqrDistance) {
                insertIndex = i;
                break;
            }
        }
        return insertIndex;
    }

    static Color RandomColour() {
        int random = Random.Range(0, 8);
        if (random == 0) return Color.blue;
        if (random == 1) return Color.cyan;
        if (random == 2) return Color.green;
        if (random == 3) return Color.grey;
        if (random == 4) return Color.magenta;
        if (random == 5) return Color.red;
        if (random == 6) return Color.white;
        if (random == 7) return Color.yellow;

        return Color.black;
    }
}
