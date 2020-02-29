using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dungeon : MonoBehaviour
{
    //public bool generateOnStart = false;
    public Area FilledArea { get; set; }
    public Area SpawnableArea { get; set; }

    public Grid grid;

    public Tilemap floorMap;
    public Tilemap wallMap;

    public List<Room> rooms;

    public FogOfWar fog;

    private IDungeonGenerator generator;
    private bool showFilledArea = false;

    // Start is called before the first frame update
    void Start() {
        generator = GetComponent<IDungeonGenerator>();
        Debug.Log(generator.ToString());

        //if (generateOnStart) {
        //    generator.AttemptToGenerateDungeon(this);
        //}
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.C)) { //sCan
            AstarPath.active.Scan();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            showFilledArea = true;
            Invoke("ResetShowFilledArea", 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.G)) { //Generate
            generator.AttemptToGenerateDungeon(this);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            showFilledArea = true;
            Invoke("ResetShowFilledArea", 1.5f);
        }
    }

    public void Scan() {
        //AstarPath.active.Scan();
    }

    public Vector2Int RandomSpawnablePosition() {
        
        Vector2Int position = Vector2Int.zero;

        int loopProtection = 100;
        bool found = false;

        while(!found && loopProtection-- > 0){
            int x = Random.Range(0, FilledArea.size);
            int y = Random.Range(0, FilledArea.size);

            if (SpawnableArea.GetPoint(x,y)) {
                //Found a free space
                position = new Vector2Int(x, y);
                found = true;
            }
        }
        return position;
    }

    public IEnumerator GenerateDungeon() {
        yield return StartCoroutine(generator.GenerateDungeon(this));
        fog.Initialise(this);
        
    }

    /// <summary>
    /// Returns the Room found at position. Returns null if no such room exists.
    /// </summary>
    public Room GetRoomOfPosition(Vector2Int position) {
        return GetRoomOfPosition(position.ToVector3Int());
    }
    public Room GetRoomOfPosition(Vector3Int position) {
        foreach (Room room in rooms) {
            if (room.Bounds.Contains(position)) {
                return room;
            }
        }
        return null;
    }

    private void OnDrawGizmos() {
        if (!showFilledArea) return;

        bool[,] area = FilledArea.GetArea();

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
