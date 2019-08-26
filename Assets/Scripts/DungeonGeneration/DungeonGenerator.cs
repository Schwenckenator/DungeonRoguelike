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
    public Tilemap floorMap;
    public Tilemap wallMap;
    //public TileBase floorTile;
    //public TileBase wallTile;
    //public Texture2D design;
    public int roomsPerLevel;
    public RoomList roomContainer;
    public List<TileInfo> tilePairs;

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel(roomsPerLevel);

        Invoke("Scan", 0.2f);
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
    }
    /// <summary>
    /// Generates an entire level of map
    /// </summary>
    void GenerateLevel(int numberOfRooms) {
        Vector2Int offset = Vector2Int.zero;
        for(int i=0; i<numberOfRooms; i++) {
            offset = new Vector2Int(i*24,0);
            int roomID = Random.Range(0, roomContainer.rooms.Length);

            GenerateRoom(roomContainer.rooms[roomID], TileLayer.collision, offset, wallMap, tilePairs);
            GenerateRoom(roomContainer.rooms[roomID], TileLayer.noCollision, offset, floorMap, tilePairs);
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
