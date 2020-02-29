using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogClearer : MonoBehaviour
{
    private Dungeon dungeon;
    private Room currentRoom;

    // Start is called before the first frame update
    private void Start()
    {
        dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
        currentRoom = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
        Debug.Log(currentRoom);
        FogOfWar.Instance.AddClearer(this);
        FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
    }

    private void Update() {
        // Is current room null? Search for your room
        if(currentRoom == null) {
            currentRoom = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
            if(currentRoom != null) {
                FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
            }
            return;
        }

        // Are you no longer in the current room?
        if (!currentRoom.Bounds.Contains(transform.position.RoundToInt())){

            FogOfWar.Instance.OnFogClearerLeaveRoom(currentRoom);

            currentRoom = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
            Debug.Log(currentRoom);
            if(currentRoom != null) {
                FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
            }
        } 
    }

    public bool IsInRoom(Room room) {
        return room.Bounds.Contains(transform.position.RoundToInt());
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
