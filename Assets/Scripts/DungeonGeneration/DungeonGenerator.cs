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
    private readonly int size;
    private readonly int increaseInterval;
    private bool[,] filled;

    public Area(int size, int increaseInterval) {
        this.size = size;
        this.increaseInterval = increaseInterval;
        filled = new bool[size, size];
    }

    public bool Filled(int x, int y) {
        if(x > filled.GetUpperBound(0) || y > filled.GetUpperBound(1)) {
            return false;
        }
        return filled[x, y];
    }

    public void SetFilled(bool fill, int minX, int minY, int maxX, int maxY) {
        if(maxX < minX || maxY < minY) {
            Debug.LogError("MIN is bigger than MAX, swap them around!");
            return;
        }
        
        //If the fill setting is larger than the bounds
        if(filled.GetUpperBound(0) < maxX || filled.GetUpperBound(1) < maxY) {
            ExpandArray();
        }

        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {
                filled[x, y] = fill;
            }
        }
    }

    public int GetSize() {
        return filled.GetUpperBound(0);
    }
    private void ExpandArray() {
        int newSize = size + increaseInterval;
        bool[,] temp = new bool[newSize, newSize];

        Debug.Log($"Current Bounds are {filled.GetUpperBound(0)}, {filled.GetUpperBound(1)}.");

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                temp[x, y] = filled[x, y];
            }
        }

        filled = temp;

        Debug.Log($"New Bounds are {filled.GetUpperBound(0)}, {filled.GetUpperBound(1)}.");
    }
}

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap floorMap;
    public Tilemap wallMap;
    public int roomsPerLevel;
    public RoomList roomContainer;
    public List<TileInfo> tilePairs;

    private Area dungeonArea;

    // Start is called before the first frame update
    void Start()
    {

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
        if (Input.GetKeyDown(KeyCode.G)) {
            NewLevel();
        }
    }

    void NewLevel() {
        dungeonArea = new Area(48, 24);
        GenerateLevel(roomsPerLevel);

        Invoke("Scan", 0.2f);
    }
    /// <summary>
    /// Generates an entire level of map
    /// </summary>
    void GenerateLevel(int numberOfRooms) {
        Vector2Int offset = Vector2Int.zero;
        int previousX = dungeonArea.GetSize() / 2;
        int previousY = dungeonArea.GetSize() / 2;

        for (int i=0; i<numberOfRooms; i++) {
            
            int roomID = Random.Range(0, roomContainer.rooms.Length);
            //Remember its place origin
            int[] index = { previousX, previousY };

            bool placeFound = false;
            int infiniteLoopProtector = 1000;

            int scanDirection = Random.Range(0, 4); //Right, Up, Left, Down

            while (!placeFound) {
                Debug.Log($"Checking square {index[0]}, {index[1]}.");
                if (!dungeonArea.Filled(index[0], index[1]) && 
                    
                    !dungeonArea.Filled(index[0] + roomContainer.rooms[roomID].width, 
                    index[1] + roomContainer.rooms[roomID].height)) 
                {

                    offset = new Vector2Int(index[0], index[1]);
                    placeFound = true;

                } else {
                    //Scan in direction
                    if (scanDirection == 0) { // Right
                        index[0]++;
                    } else if (scanDirection == 1) { // Up
                        index[1]++;
                    } else if (scanDirection == 2) { // Left
                        index[0]--;
                    } else if (scanDirection == 3) { // Down
                        index[1]--;
                    }
                }

                //Protect against infinite loops
                if (infiniteLoopProtector-- <= 0) break;
            }

            GenerateRoom(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);
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

    //int[,] GeneratePixelArray(Texture2D image, Color colour, bool avoidColour) {
    //    int[,] newMap = new int[image.width + 1, image.width + 1];

    //    for (int x = 0; x < image.width; x++) {
    //        for (int y = 0; y < image.width; y++) {

    //            Color currentColour = image.GetPixel(x, y);
    //            if (currentColour == colour && !avoidColour) {
    //                newMap[x, y] = 1;
    //            } else {
    //                newMap[x, y] = 0;
    //            }
                
    //        }
    //    }

    //    return newMap;
    //}

    //int[,] GenerateMapArray(int xSize, int ySize, bool filled) {
    //    int[,] newMap = new int[xSize, ySize];
    //    int defaultValue = filled ? 1 : 0;

    //    for (int x = 0; x < xSize; x++) {
    //        for (int y = 0; y < ySize; y++) {
    //            newMap[x, y] = defaultValue;
    //        }
    //    }

    //    return newMap;
    //}
    //int[,] GenerateMapArray(Texture2D image) {
    //    int[,] newMap = new int[image.width+1, image.width+1];

    //    for (int x = 0; x < image.width; x++) {
    //        for (int y = 0; y < image.width; y++) {

    //            bool draw = image.GetPixel(x, y).a != 0;

    //            newMap[x, y] = draw ? 1 : 0;
    //        }
    //    }

    //    return newMap;
    //}
    //int[,] GenerateMapArray(Texture2D image, bool opposite) {
    //    int[,] newMap = new int[image.width + 1, image.width + 1];

    //    for (int x = 0; x < image.width; x++) {
    //        for (int y = 0; y < image.width; y++) {

    //            bool draw = image.GetPixel(x, y).a == 0;

    //            newMap[x, y] = draw ? 1 : 0;
    //        }
    //    }

    //    return newMap;
    //}

    //void RenderMap(int[,] map, Tilemap tileMap, TileBase tile){
    //   for(int x = 0; x < map.GetUpperBound(0); x++) {
    //        for( int y = 0; y< map.GetUpperBound(1); y++) {
    //            if(map[x,y] == 1) {
    //                tileMap.SetTile(new Vector3Int(x, y, 0), tile);
    //            }
    //        }
    //    }
    //}
}
