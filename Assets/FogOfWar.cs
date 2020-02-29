using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FogState { undiscovered, discovered, visible }

public class FogOfWar : MonoBehaviour
{
    public Tilemap fogTiles;
    public Tile undiscoveredTile;
    public Tile discoveredTile;
    bool[,] fogMap;
    private int size;

    private Dictionary<FogState, Tile> fogDict;
    
    // Start is called before the first frame update
    public void Initialise(Dungeon dungeon) {
        size = dungeon.FilledArea.size;
        //fogMap = new bool[size, size];

        ////Fog is default true
        //for (int x = 0; x < size; x++) {
        //    for(int y = 0; y < size; y++) {
        //        fogMap[x, y] = true;
        //    }
        //}

        fogDict = new Dictionary<FogState, Tile> {
            { FogState.visible, null },
            { FogState.discovered, discoveredTile },
            { FogState.undiscovered, undiscoveredTile }
        };

        SetFog(FogState.undiscovered);
    }


    public void SetFog(FogState state) {
        BoundsInt bounds = new BoundsInt(Vector3Int.zero, new Vector3Int(size, size, 1));
        SetFog(bounds, state);
    }

    public void SetFog(BoundsInt bounds, FogState state) {
        TileBase[] tiles = new TileBase[size * size].Populate(fogDict[state]);
        fogTiles.SetTilesBlock(bounds, tiles);
    }

}
