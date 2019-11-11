using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A dungeon generator that generates adjacent rooms, with no need for long hallways
/// </summary>
public class DungeonGeneratorAdjacent : MonoBehaviour, IDungeonGenerator {

    public int roomsPerLevel;
    public int maxSize;
    public Tilemap floorMap;
    public Tilemap wallMap;

    public RoomList roomContainer;
    public List<TileInfo> tilePairs;

    public int maxConnections;

    public int maxSteps;

    public Dungeon dungeon;

    private Area dungeonArea;
    private Area spawnableArea;

    private Vector2Int mapCentre;
    private Vector2Int previousPosition;
    private List<Room> rooms;

    private bool isLevelGeneratorRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        dungeonArea = new Area(maxSize);
        spawnableArea = new Area(maxSize);

        mapCentre = new Vector2Int(dungeonArea.size / 2, dungeonArea.size / 2);
        previousPosition = mapCentre;


        rooms = new List<Room>();
    }

    void IDungeonGenerator.AttemptToGenerateDungeon(Dungeon dungeon) {
        if (isLevelGeneratorRunning) {
            Debug.Log("Level generator already running! Aborting.");
            return;
        }
        StartCoroutine(GenerateDungeon(roomsPerLevel, dungeon));
    }

    IEnumerator GenerateDungeon(int numOfRooms, Dungeon newDungeon) {
        isLevelGeneratorRunning = true;

        yield return StartCoroutine(GenerateRooms(numOfRooms));
        yield return StartCoroutine(GenerateHallways());
        yield return StartCoroutine(GenerateFloor());

        newDungeon.SpawnableArea = spawnableArea;
        newDungeon.FilledArea = dungeonArea;

        isLevelGeneratorRunning = false;

        dungeon.Invoke("Scan", 0.2f);
    }

    IEnumerator GenerateRooms(int numOfRooms) {
        //Using a different approach
        //Generate first room in the centre
        //Add it to a candidate list
        //From the candidate list, pick a non-filled direction
        //Generate a new room, add it to the candidate list
        //If a room has more connections than maximum, remove it from the cadidate list
        Debug.Log("Generating Rooms.");
        List<Room> candidates = new List<Room>();
        

        for(int i = 0; i < numOfRooms; i++) {
            Debug.Log($"Making room no. {i}.");
            Vector2Int newRoomCentre = Vector2Int.zero;
            int roomID = Random.Range(0, roomContainer.rooms.Length);
            Vector2Int newRoomSize = new Vector2Int(roomContainer.rooms[roomID].width, roomContainer.rooms[roomID].height);
            Room parentRoom = null;

            if (i != 0) {
                Debug.Log("Not the first room.");
                //From the candidate list, pick a non-filled direction
                if (candidates.Count <= 0) {
                    Debug.LogError("Candidate list has no candidates!");
                    break;
                }
                //Choose a candidate at random
                parentRoom = candidates.RandomItem();
                bool placeFound = false;
                Vector2Int direction = RandomExtension.CardinalDirection(); // Picks a random cardinal direction
                int loopProtection = 4;
                while (!placeFound && loopProtection-- > 0) {
                    Debug.Log("New room place not found.");
                    Debug.Log($"{loopProtection} loops remaining.");
                    //Checks the nominated direction for a room
                    
                    Vector2Int newCentre = Vector2Int.zero;
                    //HERE COMES THE DODGY SHIT
                    if (direction == Vector2Int.right) {
                        newCentre = new Vector2Int(parentRoom.Centre.x + (parentRoom.Bounds.size.x + newRoomSize.x) / 2, parentRoom.Centre.y);
                    } else if (direction == Vector2Int.up) {
                        newCentre = new Vector2Int(parentRoom.Centre.x, parentRoom.Centre.y + (parentRoom.Bounds.size.y + newRoomSize.y) / 2);
                    } else if (direction == Vector2Int.left) {
                        newCentre = new Vector2Int(parentRoom.Centre.x - (parentRoom.Bounds.size.x + newRoomSize.x) / 2, parentRoom.Centre.y);
                    } else if (direction == Vector2Int.down) {
                        newCentre = new Vector2Int(parentRoom.Centre.x, parentRoom.Centre.y - (parentRoom.Bounds.size.y + newRoomSize.y) / 2);
                    }

                    //Vector2Int min = new Vector2Int(newCentre.x - newRoomSize.x/2, newCentre.y - newRoomSize.y/2);
                    Vector2Int min = newCentre - newRoomSize.DivideByScalar(2);
                    Vector2Int max = min + newRoomSize;

                    /*What do I need?
                     *   When it's going right, I need it to use the centre of origin room for y of the new room centre,
                     *   from there take half room size y for min value
                     *   and the max point +1 of the origin room for minimum 
                     */
                    if (dungeonArea.IsFilled(min, max)) {
                        //If filled, rotate 
                        direction = direction.Rotate90();
                    } else {
                        placeFound = true;
                        newRoomCentre = newCentre;
                    }
                    yield return null;
                }

                if (!placeFound) {
                    // It didn't work for whatever reason, skip the room
                    Debug.Log("Did NOT find a suitable location. Skipping and removing from candidates.");
                    candidates.Remove(parentRoom);
                    i--; // Don't count this attempt
                    continue;
                }
                
                //Check to see if the picked room has met its neigbour limit
                if(parentRoom.ConnectionCount >= maxConnections) {
                    candidates.Remove(parentRoom);
                }

            } else {
                Debug.Log("Is the first room!");
                //place room in centre
                newRoomCentre = new Vector2Int(maxSize/ 2, maxSize / 2);
            }
            //Offset has been decided
            Room newRoom = new Room(newRoomCentre, newRoomSize);
            rooms.Add(newRoom);
            candidates.Add(newRoom);

            if(parentRoom != null) {
                parentRoom.Connect(newRoom);
                DebugDraw.Line(parentRoom.Centre.ToVector3Int(), newRoom.Centre.ToVector3Int(), Color.cyan);
            }

            Vector2Int offset = newRoomCentre - newRoomSize.DivideByScalar(2);
            DrawRoomTiles(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);

            yield return null;
        }

        
    }

    IEnumerator GenerateHallways() {
        //Recursive function
        //Start with centre room, punch hallways with all children. move outwards, repeat process
        Debug.Log("Generating hallways!");

        List<Room> candidates = new List<Room> {
            rooms[0]
        };

        while(candidates.Count > 0) {
            //Debug.Log("Has a candidate.");
            //Pops first candidate
            Room parentRoom = candidates[0];
            candidates.RemoveAt(0);

            foreach(Room childRoom in parentRoom.Children) {
                //Punch a hallway here. HUGE one fuck it why not
                //Vector2Int[] tilesToClear = DesignateHallwayTiles(parentRoom.Centre, childRoom.Centre);
                Vector2Int[] tilesToClear = DesignateDoorTiles(parentRoom, childRoom);

                DrawTiles(tilesToClear, wallMap, null);
                candidates.Add(childRoom);
            }
            

            yield return null; // Protects from crashing editor
        }

        yield return null;
    }

    IEnumerator GenerateFloor() {

        bool[,] area = dungeonArea.GetArea();

        for (int x = 0; x < area.GetUpperBound(0); x++) {
            for (int y = 0; y < area.GetUpperBound(1); y++) {
                if (area[x, y] && wallMap.GetTile(new Vector3Int(x, y, 0)) == null) {
                    floorMap.SetTile(new Vector3Int(x, y, 0), tilePairs[0].tile);
                    spawnableArea.SetPoint(true, x, y);
                }
            }
        }
        yield return null;

    }

    void DrawRoomTiles(Texture2D image, TileLayer layer, Vector2Int offset, Tilemap tileMap, List<TileInfo> tiles) {
        Debug.Log("Drawing Room Tiles.");
        for (int x = 0; x < image.width; x++) {
            for (int y = 0; y < image.height; y++) {

                Color currentColour = image.GetPixel(x, y);
                //Debug.Log(currentColour.ToString());
                TileInfo tileInfo = null;

                foreach (var tile in tiles) {
                    if (currentColour == tile.colour) {

                        tileInfo = tile;
                        //Debug.Log("Tile found.");
                        break;
                    }
                }
                if (tileInfo == null) {
                    Debug.LogError("TileInfo is null!");
                }
                if (layer == tileInfo.layer) {
                    tileMap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), tileInfo.tile);
                }
            }
        }

        dungeonArea.SetArea(true, offset.x, offset.y, offset.x + image.width, offset.y + image.height);


    }

    void DrawTiles(Vector2Int[] tilesToDraw, Tilemap tileMap, TileBase tileGraphic) {
        foreach(var tile in tilesToDraw) {
            tileMap.SetTile(new Vector3Int(tile.x, tile.y, 0), tileGraphic);
        }
    }


    Vector2Int[] DesignateDoorTiles(Room origin, Room target) {
        List<Vector2Int> newTiles = new List<Vector2Int>();

        Vector2Int direction = target.Centre - origin.Centre;

        Debug.Log($"Vectors subtracted! {target.Centre} - {origin.Centre} = {direction}");

        direction.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));

        Debug.Log($"Vector Clamped! {direction}");

        int loopProtection = 100;
   
        Vector2Int currentSquare = origin.Centre;
        Room currentRoom = origin;
        int completedRooms = 0;
        //bool inWallTiles = false;
        while (completedRooms < 2 && loopProtection-- > 0) {
            Debug.Log($"Hallway loops remaining: {loopProtection}.");
            Debug.Log($"Current square is now {currentSquare}, target is {target.Centre}.");
            Debug.Log($"Current rooms bounds are {currentRoom.Bounds.min} & {currentRoom.Bounds.max}.");

            //TEST BOUNDS
            Debug.Log($"Is {currentSquare.ToVector3Int()} in {currentRoom.Bounds}? {currentRoom.Bounds.Contains(currentSquare.ToVector3Int())}.");



            if (!currentRoom.Bounds.Contains(currentSquare.ToVector3Int())) {
                Debug.Log("Left current room, let's designate tiles!");
                //Out of the room, cut!
                if(wallMap.GetTile(currentSquare.ToVector3Int()) != null) {
                    Debug.Log($"Adding {currentSquare} to list.");
                    newTiles.Add(currentSquare);
                } else {
                    //The door should be clear?
                    Debug.Log("Reversing direction!");
                    direction *= -1;
                    currentRoom = target;
                    completedRooms++;
                }
            } else {
                Debug.Log("Still inside current room.");
            }

            //newTiles.Add(currentSquare);
            currentSquare += direction;

            
            //currentSquare != target.Centre
        }
        // Travel from centre to Bounds, once out of bounds, add tiles to list until hit a null tiles
        // Turn around, once out of target room, same thing

        return newTiles.ToArray();
    }

}
