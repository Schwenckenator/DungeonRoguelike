using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FogState { undiscovered, discovered, visible }

public class FogOfWar : MonoBehaviour
{
    public static FogOfWar Instance { get; private set; }

    public Tilemap fogTiles;
    public Tile undiscoveredTile;
    public Tile discoveredTile;
    private int size;

    private Dictionary<FogState, Tile> fogDict;
    private List<FogClearer> clearers;
    private Dungeon dungeon;
    
    // Start is called before the first frame update
    public void Initialise(Dungeon dungeon) {
        Instance = this;
        this.dungeon = dungeon;
        size = dungeon.FilledArea.size;

        fogDict = new Dictionary<FogState, Tile> {
            { FogState.visible, null },
            { FogState.discovered, discoveredTile },
            { FogState.undiscovered, undiscoveredTile }
        };

        clearers = new List<FogClearer>();

        SetFog(FogState.undiscovered);
    }

    public void OnFogClearerEnterRoom(Room room) {
        SetFog(FogState.visible, room.Bounds);
    }

    public void OnFogClearerLeaveRoom(Room room) {
        bool roomOccupied = false;
        foreach (var clearer in clearers) {
            if (clearer.IsInRoom(room)) {
                roomOccupied = true;
                break;
            }
        }
        if (!roomOccupied) {
            SetFog(FogState.discovered, room.Bounds);
        }
    }

    public void SetFog(FogState state) {
        BoundsInt bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(size, size, 1));
        SetFog(state, bounds);
    }

    public void SetFog(FogState state, BoundsInt bounds) {
        TileBase[] tiles = new TileBase[size * size].Populate(fogDict[state]);
        fogTiles.SetTilesBlock(bounds, tiles);
    }

    public void AddClearer(FogClearer value) {
        clearers.Add(value);
    }

}
