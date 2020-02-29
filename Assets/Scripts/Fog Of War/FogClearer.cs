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
        currentRoom = dungeon.GetRoomOfPosition(transform.position);
        Debug.Log(currentRoom);
        FogOfWar.Instance.AddClearer(this);
        FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
    }

    private void Update() {
        // Is current room null? Search for your room
        if (currentRoom == null) {
            FindRoom();
            if (currentRoom != null) {
                FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
            }
            return;
        }

        // Are you no longer in the current room?
        if (!currentRoom.Contains(transform.position)){

            FogOfWar.Instance.OnFogClearerLeaveRoom(currentRoom);

            currentRoom = dungeon.GetRoomOfPosition(transform.position);
            Debug.Log(currentRoom);
            FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
        } 
    }

    public bool IsInRoom(Room room) {
        return room.Contains(transform.position);
    }

    private void FindRoom() {
        currentRoom = dungeon.GetRoomOfPosition(transform.position);
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
