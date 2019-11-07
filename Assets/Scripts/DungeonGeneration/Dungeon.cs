using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dungeon : MonoBehaviour
{
    private Area filledArea;
    private Area spawnableArea;

    private Grid grid;

    public Tilemap floorMap;
    public Tilemap wallMap;

    public IDungeonGenerator generator;

    private bool showFilledArea = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) { //sCan
            AstarPath.active.Scan();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            showFilledArea = true;
            Invoke("ResetShowFilledArea", 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.G)) { //Generate
            generator.AttemptToGenerateDungeon();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            showFilledArea = true;
            Invoke("ResetShowFilledArea", 1.5f);
        }


    }

    void Scan() {
        AstarPath.active.Scan();
    }

    private void OnDrawGizmos() {
        if (!showFilledArea) return;

        bool[,] area = filledArea.GetArea();

        for (int x = 0; x < area.GetUpperBound(0); x++) {
            for (int y = 0; y < area.GetUpperBound(1); y++) {
                if (area[x, y]) {
                    Vector2 centre = new Vector2(x + 0.5f, y + 0.5f);
                    Gizmos.DrawWireSphere(centre, 0.5f);
                }
            }
        }
    }


}
