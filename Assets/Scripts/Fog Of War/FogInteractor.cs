using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FogInteractor : MonoBehaviour
{
    protected Dungeon dungeon;
    protected Room currentRoom;
    protected bool roomFound = false;

    //public abstract void Initialise();
    protected abstract void ChangedRoom();

    public virtual void Initialise() {
        Debug.Log("Fog Interator Start!");
        dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
        currentRoom = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
        Debug.Log(currentRoom);
        //Initialise();
    }

    protected void Update() {
        if (currentRoom == null) {
            currentRoom = FindCurrentRoom();
        }

        if (!roomFound && currentRoom != null) {
            FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
            roomFound = true;
        }

        // Are you no longer in the current room?
        if (!currentRoom.Contains(transform.position.RoundToInt())) {
            roomFound = false;
            ChangedRoom();
        }
    }

    protected Room FindCurrentRoom() {
        return dungeon.GetRoomOfPosition(transform.position.RoundToInt());
    }
    
}
