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

public class Area {
    public readonly int size;
    private bool[,] filled;

    public Area(int size) {
        this.size = size;
        filled = new bool[size, size];
    }

    public bool[,] GetArea() {
        return filled;
    }

    public bool IsFilled(int x, int y) {
        //Squares out of bounds count as filled
        if (x > filled.GetUpperBound(0) ||
            y > filled.GetUpperBound(1) ||
            x < filled.GetLowerBound(0) ||
            y < filled.GetLowerBound(1)) {
            return true;
        }
        return filled[x, y];
    }
    public bool IsFilled(Vector2Int cell) {
        return IsFilled(cell.x, cell.y);
    }

    /// <summary>
    /// Very expensive if it's not filled, I should optimise this
    /// </summary>
    /// <returns>Returns true if any square inside is filled</returns>
    public bool IsFilled(int minX, int minY, int maxX, int maxY) {
        Debug.Log($"Checking if area filled, min {minX},{minY}; max {maxX},{maxY}.");
        for (int x = minX; x < maxX; x++) {
            for(int y = minY; y < maxY; y++) {
                
                Debug.DrawLine(new Vector2(x-0.5f, y - 0.5f), new Vector2(x + 0.5f, y + 0.5f), Color.red, 3f);
                if (IsFilled(x,y)) return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Very expensive if it's not filled, I should optimise this
    /// </summary>
    /// <returns>Returns true if any square inside is filled</returns>
    public bool IsFilled(Vector2Int min, Vector2Int max) {
        return IsFilled(min.x, min.y, max.x, max.y);
    }

    public void SetFilled(bool fill, int minX, int minY, int maxX, int maxY) {
        if(maxX < minX || maxY < minY) {
            Debug.LogError("MIN is bigger than MAX, swap them around!");
            return;
        }

        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {
                filled[x, y] = fill;
            }
        }
    }
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

    // Start is called before the first frame update
    void Start()
    {
        dungeonArea = new Area(maxSize);

        mapCentre = new Vector2Int(dungeonArea.size / 2, dungeonArea.size / 2);
        previousPosition = mapCentre;
    }

    void Scan() {
        AstarPath.active.Scan();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            AstarPath.active.Scan();
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            AddRooms(1);
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            AddRooms(roomsPerLevel);
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
                    Vector2 centre = new Vector2(x, y);
                    Gizmos.DrawWireSphere(centre, 0.5f);
                }
            }
        }
    }

    void ResetShowFilledArea() {
        showFilledArea = false;
    }

    void AddRooms(int rooms) {
        
        GenerateLevel(rooms);

        Invoke("Scan", 0.2f);
    }
    /// <summary>
    /// Generates an entire level of map
    /// </summary>
    void GenerateLevel(int numberOfRooms) {
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


    bool CheckCornersEmpty(Vector2Int origin, int width, int height) {
        Vector2 drawOrigin = origin;
        //Draw checking box
        Debug.DrawLine(drawOrigin, new Vector2(origin.x, origin.y + height), Color.red, 3f);
        Debug.DrawLine(drawOrigin, new Vector2(origin.x + width, origin.y), Color.red, 3f);
        Debug.DrawLine(new Vector2(origin.x + width, origin.y), new Vector2(origin.x + width, origin.y + height), Color.red, 3f);
        Debug.DrawLine(new Vector2(origin.x, origin.y + height), new Vector2(origin.x + width, origin.y + height), Color.red, 3f);

        //bottom left
        if (dungeonArea.IsFilled(origin.x, origin.y)) return false;
        //bottom right
        if (dungeonArea.IsFilled(origin.x+width-1, origin.y)) return false;
        //top left
        if (dungeonArea.IsFilled(origin.x, origin.y+height-1)) return false;
        //top right
        if (dungeonArea.IsFilled(origin.x+width-1, origin.y+height-1)) return false;

        
        //If it gets here, it's all free!
        return true;
    }
}
