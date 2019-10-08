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
    private List<Room> rooms;
    

    // Start is called before the first frame update
    void Start()
    {
        dungeonArea = new Area(maxSize);

        mapCentre = new Vector2Int(dungeonArea.size / 2, dungeonArea.size / 2);
        previousPosition = mapCentre;

        
        rooms = new List<Room>();
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
        if (Input.GetKeyDown(KeyCode.T)) {
            RunTests();
        }
    }

    private void RunTests() {


        //Directly UP
        Debug.Log("Up Test, should see x=0, y=1");
        Debug.Log(ApproximateDirection(Vector2.right, Vector2.right + Vector2.up).ToString());

        //Directly RIGHT
        Debug.Log("Right Test, should see x=1, y=0");
        Debug.Log(ApproximateDirection(Vector2.right, Vector2.right * 2).ToString());

        //Directly DOWN
        Debug.Log("Down Test, should see x=0, y=-1");
        Debug.Log(ApproximateDirection(Vector2.up, Vector2.down).ToString());

        //Directly LEFT
        Debug.Log("Left Test, should see x=-1, y=0");
        Debug.Log(ApproximateDirection(Vector2.right, Vector2.left).ToString());

        //Slightly RIGHT

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

    void AddRoomsWanderer(int roomCount) {
        
        GenerateLevelWanderer(roomCount);

        Invoke("Scan", 0.2f);
    }
    void AddRoomsRandom(int roomCount) {

        StartCoroutine(GenerateLevelRandom(roomCount));
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
                DrawRoomTiles(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);
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
                    rooms.Add(new Room(centre, roomSize));
                }

                //Protect against infinite loops
                if (infiniteLoopProtector-- <= 0) break;
            }
            if (placeFound) {
                DrawRoomTiles(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);
            } else {
                Debug.Log("Failed to generate room.");
            }
            yield return null;
            //GenerateRoom(roomContainer.rooms[roomID], TileLayer.noCollision, offset, floorMap, tilePairs);
        }
        Debug.Log($"Dungeon Generated Randomly! {generatedRooms} rooms successfully generated.");

        StartCoroutine(GenerateConnectionsShortestWalking());
    }

    IEnumerator GenerateConnectionsShortestWalking() {
        Debug.Log("Starting 'Shortest Walking Algorithm'.");
        //Pick the starting room at random
        List<Room> roomCandidates = new List<Room> { rooms[0] };

        bool unconnectedRoomsExist = true;

        Color walkColour = Color.cyan;

        

        while (unconnectedRoomsExist) {
            Debug.Log("Looking for new connection...");
            Room origin = null;
            Room nearestNeighbour = null;
            //Large number
            float nearestSqrDistance = 1000000000;

            foreach (Room candidate in roomCandidates) {
                foreach (Room neighbour in rooms) {
                    Debug.Log($"Checking candidate {candidate.Centre} against room {neighbour.Centre}.");
                    if (candidate == neighbour) {
                        Debug.Log($"Is itself, skipping.");
                        continue;
                    }
                    if (candidate.Neighbours.Contains(neighbour)) {
                        Debug.Log("Already connected to this room. Skipping.");
                        continue;
                    }
                    if (neighbour.ConnectionCount >= maxConnections) {
                        Debug.Log($"Neighbour {neighbour.Centre} has {neighbour.ConnectionCount}, more or equal to maximum connections {maxConnections}.");
                        continue;
                    }


                    if(candidate.SqrDistance(neighbour) < nearestSqrDistance) {
                        Debug.Log("This is the new nearest Connection!");
                        origin = candidate;
                        nearestNeighbour = neighbour;
                        nearestSqrDistance = candidate.SqrDistance(neighbour);
                        Debug.Log($"Nearest sqrDistance is now {nearestSqrDistance}.");
                    } else {
                        Debug.Log($"SqrDistance {candidate.SqrDistance(neighbour)} is longer than {nearestSqrDistance}.");
                    }
                }
            }

            if(origin == null || nearestNeighbour == null) {
                Debug.Log("Connection attempt failed!");
            } else {
                Debug.Log($"Connection from {origin.Centre} to {nearestNeighbour.Centre} made.");
                DrawDebugHallway(origin.Centre, nearestNeighbour.Centre, walkColour);
                origin.Connect(nearestNeighbour);


                if (!roomCandidates.Contains(nearestNeighbour) && nearestNeighbour.ConnectionCount <= maxConnections) {
                    roomCandidates.Add(nearestNeighbour);
                }
            }

            //Make path to that neighbour

            List<Room> removalCandidates = new List<Room>();
            foreach (Room room in roomCandidates) {
                if (room.ConnectionCount >= maxConnections) {
                    removalCandidates.Add(room);
                }
            }
            foreach (Room room in removalCandidates) {
                Debug.Log($"Removing {room.Centre} from candidates.");
                roomCandidates.Remove(room);
            }

            unconnectedRoomsExist = false;
            foreach(Room room in rooms) {
                if(room.ConnectionCount == 0) {
                    unconnectedRoomsExist = true;
                    break;
                }
            }

            yield return null;
            //yield return new WaitForSeconds(0.25f);
        }

        Debug.Log("Room Connections successfully generated!");
        StartCoroutine(GenerateHallways());

    }

    IEnumerator GenerateHallways() {

        List<Room> finishedRooms = new List<Room>();

        foreach(var room in rooms) {
            foreach(var neighbour in room.Neighbours) {
                //Find approximate direction to neighbour centres
                Vector2 approxDir = ApproximateDirection(room.Centre, neighbour.Centre);
                
                //Find halfway point between centres
                Vector2Int origin = Vector2Int.zero;
                Vector2Int target = Vector2Int.zero;

                //Kinda gross, not good enough at maths to simplify yet
                if (approxDir == Vector2.right) {
                    int y = (room.Centre.y - neighbour.Centre.y) / 2;
                    int xStart = room.Centre.x + room.Bounds.xMax;
                    int xEnd = neighbour.Centre.x + neighbour.Bounds.xMin;
                    origin = new Vector2Int(xStart, y);
                    target = new Vector2Int(xEnd, y);

                } else if (approxDir == Vector2.left) { 
                    int y = (room.Centre.y - neighbour.Centre.y) / 2;
                    int xStart = room.Centre.x + room.Bounds.xMin;
                    int xEnd = neighbour.Centre.x + neighbour.Bounds.xMax;
                    origin = new Vector2Int(xStart, y);
                    target = new Vector2Int(xEnd, y);

                } else if (approxDir == Vector2.up) {
                    int x = (room.Centre.x - neighbour.Centre.x) / 2;
                    int yStart = room.Bounds.yMax;
                    int yEnd = neighbour.Bounds.yMin;
                    origin = new Vector2Int(x, yStart);
                    target = new Vector2Int(x, yEnd);

                } else if (approxDir == Vector2.down) {
                    int x = (room.Centre.y - neighbour.Centre.y) / 2;
                    int yStart = room.Bounds.yMin;
                    int yEnd = neighbour.Bounds.yMax;
                    origin = new Vector2Int(x, yStart);
                    target = new Vector2Int(x, yEnd);
                }

                DrawDebugHallway(origin, target, Color.red);


                //DrawHallwayTiles(origin, target, wallMap, 1);
                //Stop when it intersects
                yield return null;
            }
        }


        throw new System.NotImplementedException();
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
    void DrawRoomTiles(Texture2D image, TileLayer layer, Vector2Int offset, Tilemap tileMap, List<TileInfo> tiles) {

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

    void DrawHallwayTiles(Vector2Int[] tiles, Tilemap tilemap, int width) {
        //Decide the tiles to be filled in
        // in this test case, fill in tiles either side of the line, allowing a one tile path in the middle.

        // Hallways only go in straight lines, with 90 deg turns
        //Find nearest direction, move that way, halfway through turn 90 deg, move then one more 90 deg turn

    }

    static Vector2 ApproximateDirection(Vector2 origin, Vector2 target) {

        Vector2 displacement = target - origin;

        if (displacement.sqrMagnitude <= 0) {
            return Vector2.zero;
        }

        if(displacement.x > Mathf.Abs(displacement.y)) {
            return Vector2.right;
        }
        if(-displacement.x > Mathf.Abs(displacement.y)) {
            return Vector2.left;
        }
        if (displacement.y > Mathf.Abs(displacement.x)) {
            return Vector2.up;
        }
        if(-displacement.y > Mathf.Abs(displacement.x)) {
            return Vector2.down;
        }

        return Vector2.zero;
    }
}
