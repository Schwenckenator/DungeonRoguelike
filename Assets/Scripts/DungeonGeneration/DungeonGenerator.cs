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

    private bool isLevelGeneratorRunning = false;
    

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

    void AddRoomsRandom(int roomCount) {
        if (isLevelGeneratorRunning) {
            Debug.Log("Level generator already running! Aborting.");
            return;
        }
        StartCoroutine(GenerateDungeonRandom(roomCount));
    }
    IEnumerator GenerateDungeonRandom(int roomCount) {
        isLevelGeneratorRunning = true;

        yield return StartCoroutine(GenerateLevelRandom(roomCount));
        yield return StartCoroutine(GenerateConnectionsShortestWalking());
        yield return StartCoroutine(GenerateHallways());
        yield return StartCoroutine(GenerateFloor());

        isLevelGeneratorRunning = false;

        Invoke("Scan", 0.2f);
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

        //StartCoroutine(GenerateConnectionsShortestWalking());
    }

    IEnumerator GenerateConnectionsShortestWalking() {
        Debug.Log("Starting 'Shortest Walking Algorithm'.");
        //Pick the starting room at random
        List<Room> roomCandidates = new List<Room> { rooms[0] };

        bool unconnectedRoomsExist = true;

        Color walkColour = Color.cyan;

        

        while (unconnectedRoomsExist) {
            //Debug.Log("Looking for new connection...");
            Room origin = null;
            Room nearestNeighbour = null;
            //Large number
            float nearestSqrDistance = 1000000000;

            foreach (Room candidate in roomCandidates) {
                foreach (Room neighbour in rooms) {
                    //Debug.Log($"Checking candidate {candidate.Centre} against room {neighbour.Centre}.");
                    if (candidate == neighbour) {
                        //Debug.Log($"Is itself, skipping.");
                        continue;
                    }
                    if (candidate.Neighbours.Contains(neighbour)) {
                        //Debug.Log("Already connected to this room. Skipping.");
                        continue;
                    }
                    if (neighbour.ConnectionCount >= maxConnections) {
                        //Debug.Log($"Neighbour {neighbour.Centre} has {neighbour.ConnectionCount}, more or equal to maximum connections {maxConnections}.");
                        continue;
                    }


                    if (candidate.SqrDistance(neighbour) < nearestSqrDistance) {
                        //Debug.Log("This is the new nearest Connection!");
                        origin = candidate;
                        nearestNeighbour = neighbour;
                        nearestSqrDistance = candidate.SqrDistance(neighbour);
                        //Debug.Log($"Nearest sqrDistance is now {nearestSqrDistance}.");
                        //} else {
                        //    Debug.Log($"SqrDistance {candidate.SqrDistance(neighbour)} is longer than {nearestSqrDistance}.");
                        //}
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
        }

        Debug.Log("Room Connections successfully generated!");

    }

    IEnumerator GenerateHallways() {

        List<Pair<Room>> hallways = new List<Pair<Room>>();

        foreach (var room in rooms) {

            foreach (var neighbour in room.Neighbours) {
                Pair<Room> newHallway = new Pair<Room>(room, neighbour);

                if (HallwayExists(hallways, newHallway)) {
                    continue;
                }

                //Find approximate direction to neighbour centres
                Vector2 approxDir = ApproximateDirection(room.Centre, neighbour.Centre);

                //Find halfway point between centres
                Vector2Int origin = Vector2Int.zero;
                Vector2Int target = Vector2Int.zero;

                Color hallwayDebugColour = Color.black;
                Vector2Int debugHallwayMargin = new Vector2Int(Mathf.RoundToInt(approxDir.x), Mathf.RoundToInt(approxDir.y));

                //Kinda gross, not good enough at maths to simplify yet
                if (approxDir == Vector2.right) {
                    int y = (room.Centre.y + neighbour.Centre.y) / 2;
                    int xStart = room.Bounds.xMax;
                    int xEnd = neighbour.Bounds.xMin;
                    origin = new Vector2Int(xStart, y);
                    target = new Vector2Int(xEnd, y);
                    hallwayDebugColour = Color.blue;

                } else if (approxDir == Vector2.left) {
                    int y = (room.Centre.y + neighbour.Centre.y) / 2;
                    int xStart = room.Bounds.xMin;
                    int xEnd = neighbour.Bounds.xMax;
                    origin = new Vector2Int(xStart, y);
                    target = new Vector2Int(xEnd, y);
                    hallwayDebugColour = Color.yellow;

                } else if (approxDir == Vector2.up) {
                    int x = (room.Centre.x + neighbour.Centre.x) / 2;
                    int yStart = room.Bounds.yMax;
                    int yEnd = neighbour.Bounds.yMin;
                    origin = new Vector2Int(x, yStart);
                    target = new Vector2Int(x, yEnd);
                    hallwayDebugColour = Color.green;

                } else if (approxDir == Vector2.down) {
                    int x = (room.Centre.x + neighbour.Centre.x) / 2;
                    int yStart = room.Bounds.yMin;
                    int yEnd = neighbour.Bounds.yMax;
                    origin = new Vector2Int(x, yStart);
                    target = new Vector2Int(x, yEnd);
                    hallwayDebugColour = Color.magenta;

                } else {
                    throw new System.Exception("Hallway Vector has no direction!");
                }

                DrawDebugHallway(origin - debugHallwayMargin, target + debugHallwayMargin, hallwayDebugColour);

                //DesignateHallwayTiles(origin, target, approxDir, out Vector2Int[] tilesToDraw, out Vector2Int[] tilesToRemove);
                DesignateHallwayTilesNEW(origin, target, approxDir, out Vector2Int[] tilesToDraw, out Vector2Int[] tilesToRemove);

                DrawHallwayTiles(tilesToDraw, tilesToRemove, wallMap, tilePairs);

                hallways.Add(new Pair<Room>(room, neighbour));

                yield return new WaitForSeconds(1f);
            }
        }
    }

    private static bool HallwayExists(List<Pair<Room>> hallways, Pair<Room> newHallway) {
        bool hallwayExists = false;

        foreach (var hallway in hallways) {
            if (newHallway.Equals(hallway)) {
                hallwayExists = true;
                break;
            }
        }

        return hallwayExists;
    }

    IEnumerator GenerateFloor() {

        bool[,] area = dungeonArea.GetArea();

        for(int x = 0; x < area.GetUpperBound(0); x++) {
            for(int y=0; y < area.GetUpperBound(1); y++) {
                if (area[x, y] && wallMap.GetTile(new Vector3Int(x,y,0)) == null) {
                    floorMap.SetTile(new Vector3Int(x, y, 0), tilePairs[0].tile);
                }
            }
        }
        yield return null;

    }

    private void DesignateHallwayTiles(Vector2Int origin, Vector2Int target, Vector2 direction, out Vector2Int[] wallTiles, out Vector2Int[] floorTiles, int width = 1, int margin = 3) {
        List<Vector2Int> wallTileList = new List<Vector2Int>();
        List<Vector2Int> floorTileList = new List<Vector2Int>();

        //TODO: clean up repetition... Don't know how I'd do it though
        if (direction == Vector2.up || direction == Vector2.down) { //travels in y direction, 
            for (int i = Mathf.Min(origin.y, target.y) - margin; i < Mathf.Max(origin.y, target.y) + margin; i++) {
                //TODO: don't ignore width
                floorTileList.Add(new Vector2Int(origin.x, i));
            }

            for (int i = Mathf.Min(origin.y, target.y); i < Mathf.Max(origin.y, target.y); i++) {
                wallTileList.Add(new Vector2Int(origin.x + width, i));
                wallTileList.Add(new Vector2Int(origin.x - width, i));
            }

        } else if (direction == Vector2.right || direction == Vector2.left) {
            for (int i = Mathf.Min(origin.x, target.x) - margin; i < Mathf.Max(origin.x, target.x) + margin; i++) {
                //TODO: don't ignore width
                floorTileList.Add(new Vector2Int(i, origin.y));
            }

            for (int i = Mathf.Min(origin.x, target.x); i < Mathf.Max(origin.x, target.x); i++) {
                wallTileList.Add(new Vector2Int(i, origin.y + width));
                wallTileList.Add(new Vector2Int(i, origin.y - width));
            }

        }
        wallTiles = wallTileList.ToArray();
        floorTiles = floorTileList.ToArray();
    }

    void DesignateHallwayTilesNEW(Vector2Int origin, Vector2Int target, Vector2 direction, out Vector2Int[] wallTiles, out Vector2Int[] floorTiles, int width = 1) {
        List<Vector2Int> wallTileList = new List<Vector2Int>();
        List<Vector2Int> floorTileList = new List<Vector2Int>();

        if (direction == Vector2.up || direction == Vector2.down) {
            bool wallHit = false;
            bool breachedRoom = false;
            Vector3Int hallwayCentre = new Vector3Int((origin.x + target.x) / 2, (origin.y + target.y) / 2, 0);
            Vector3Int currentSquare = hallwayCentre;
            Vector2Int currentDirection = direction.RoundToInt();
            while (!wallHit && !breachedRoom) {
                


                //If you already hit a wall and come to a blank tile
                if (wallHit && wallMap.GetTile(currentSquare) == null) {
                    //The hallway has successfully breached the room
                    breachedRoom = true;
                }
                // if you hit a tile that isn't empty
                if (wallMap.GetTile(currentSquare) != null) {
                    //You have hit a wall, probably
                    wallHit = true;
                }

                //At current square
                floorTileList.Add(currentSquare.ToVector2Int());
                wallTileList.Add(currentSquare.ToVector2Int() + Vector2Int.left * width);
                wallTileList.Add(currentSquare.ToVector2Int() + Vector2Int.right * width);
                currentSquare += currentDirection.ToVector3Int();

            }
            //Reverse direction and repeat
            currentDirection *= -1;
            wallHit = false;
            breachedRoom = false;
            currentSquare = hallwayCentre;


            while (!wallHit && !breachedRoom) {
                //If you already hit a wall and come to a blank tile
                if (wallHit && wallMap.GetTile(currentSquare) == null) {
                    //The hallway has successfully breached the room
                    breachedRoom = true;
                }
                // if you hit a tile that isn't empty
                if (wallMap.GetTile(currentSquare) != null) {
                    //You have hit a wall, probably
                    wallHit = true;
                }
                //At current square
                floorTileList.Add(currentSquare.ToVector2Int());
                wallTileList.Add(currentSquare.ToVector2Int() + Vector2Int.left * width);
                wallTileList.Add(currentSquare.ToVector2Int() + Vector2Int.right * width);
                currentSquare += currentDirection.ToVector3Int();
            }

        } else if (direction == Vector2.right || direction == Vector2.left) {

        }


        wallTiles = wallTileList.ToArray();
        floorTiles = floorTileList.ToArray();
    }

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

    void DrawHallwayTiles(Vector2Int[] drawTiles, Vector2Int[] removeTiles, Tilemap tileMap, List<TileInfo> tiles) {
        foreach(var tile in drawTiles) {
            tileMap.SetTile(new Vector3Int(tile.x, tile.y, 0), tiles[1].tile); //TODO: fix this [1] bad bad ju ju
            dungeonArea.SetFilled(true, tile);
        }
        foreach(var tile in removeTiles) {
            tileMap.SetTile(new Vector3Int(tile.x, tile.y, 0), null);
            Vector2 pos = tile;
            Debug.DrawLine(new Vector2(pos.x, pos.y), new Vector2(pos.x + 1, pos.y + 1), Color.magenta, 100f);
            Debug.DrawLine(new Vector2(pos.x, pos.y), new Vector2(pos.x + 1, pos.y + 1), Color.magenta, 100f);
            dungeonArea.SetFilled(true, tile);
        }

    }

    static Vector2 ApproximateDirection(Vector2 origin, Vector2 target) {

        Vector2 displacement = target - origin;

        if (displacement.sqrMagnitude <= 0) {
            return Vector2.zero;
        }

        if(displacement.x >= Mathf.Abs(displacement.y)) {
            return Vector2.right;
        }
        if(-displacement.x >= Mathf.Abs(displacement.y)) {
            return Vector2.left;
        }
        if (displacement.y >= Mathf.Abs(displacement.x)) {
            return Vector2.up;
        }
        if(-displacement.y >= Mathf.Abs(displacement.x)) {
            return Vector2.down;
        }
        throw new System.Exception(
            $"Approximate Direction failed!\n" +
            $"Origin: {origin.ToString()}, Target: {target.ToString()}, Displacement: {displacement.ToString()}");
    }

    void DrawDebugHallway(Vector2Int value1, Vector2Int value2, Color colour) {
        Vector2 drawValue1 = value1;
        Vector2 drawValue2 = value2;
        //Connect to that room
        Debug.DrawLine(drawValue1, drawValue2, colour, 100f);
    }
}
