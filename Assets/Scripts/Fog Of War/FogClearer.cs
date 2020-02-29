using UnityEngine;

public class FogClearer : MonoBehaviour
{
    private Dungeon dungeon;
    private Room currentRoom;
    private bool roomFound = false;

    // Start is called before the first frame update
    private void Start()
    {
        dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
        currentRoom = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
        Debug.Log(currentRoom);
        FogOfWar.Instance.AddClearer(this);
        if(currentRoom != null) {
            FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
            roomFound = true;
        }
        
    }

    private void Update() {
        // Is current room null? Search for your room
        if (currentRoom == null) {
            currentRoom = FindCurrentRoom();
        }

        if(!roomFound && currentRoom != null) {
            FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
            roomFound = true;
        }

        // Are you no longer in the current room?
        if (!currentRoom.Contains(transform.position.RoundToInt())){

            FogOfWar.Instance.OnFogClearerLeaveRoom(currentRoom);
            roomFound = false;

            currentRoom = FindCurrentRoom();
            Debug.Log(currentRoom);
        } 
    }

    public bool IsInRoom(Room room) {
        return room.Contains(transform.position.RoundToInt());
    }

    private Room FindCurrentRoom() {
        return dungeon.GetRoomOfPosition(transform.position.RoundToInt());
    }
    //public void ClearFog(Room room) {
    //    //Room room = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
    //    FogOfWar.Instance.SetFog(room.Bounds, FogState.visible);
    //}
    //public void ReturnFog(Room room) {
    //    //Room room = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
    //    FogOfWar.Instance.SetFog(room.Bounds, FogState.discovered);
    //}
}
