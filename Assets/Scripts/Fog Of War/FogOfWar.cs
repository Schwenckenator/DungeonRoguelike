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
    private List<FogHidee> hidees;
    private Dungeon dungeon;

    private void Awake() {
        Instance = this;
    }
    // Start is called before the first frame update
    public void Initialise(Dungeon dungeon) {
        this.dungeon = dungeon;
        size = dungeon.FilledArea.size;

        fogDict = new Dictionary<FogState, Tile> {
            { FogState.visible, null },
            { FogState.discovered, discoveredTile },
            { FogState.undiscovered, undiscoveredTile }
        };

        clearers = new List<FogClearer>();
        hidees = new List<FogHidee>();

        SetFog(FogState.undiscovered);
        //DebugSeeAllRooms();
    }

    public void OnFogClearerEnterRoom(Room room) {
        SetFog(FogState.visible, room.Bounds);
    }

    public void OnFogClearerLeaveRoom(Room room) {
        if (!RoomOccupiedByClearer(room)) {
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

        foreach (FogHidee hidee in hidees) {
            if (bounds.Contains(hidee.transform.position.RoundToInt())) {
                hidee.SetVisible(state == FogState.visible);
            }
        }
        if(state == FogState.visible) {
            BattleController.Instance.CheckForNewMonsterAggro(bounds);
        }
    }

    public void AddClearer(FogClearer value) {
        clearers.Add(value);
    }

    public void AddHidee(FogHidee value) {
        hidees.Add(value);
    }

    public bool RoomOccupiedByClearer(Room room) {
        foreach (var clearer in clearers) {
            if (clearer.IsInRoom(room)) {
                return true;
            }
        }
        return false;
    }

    #region Debug

    private void DebugSeeAllRooms() {
        SetFog(FogState.undiscovered);
        foreach(Room room in dungeon.rooms) {
            SetFog(FogState.visible, room.Bounds);
        }
    }

    #endregion

}
