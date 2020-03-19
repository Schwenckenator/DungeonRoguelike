using UnityEngine;

public class FogClearer : FogInteractor
{
    public override void Initialise() {
        base.Initialise();
        Debug.Log($"Fog Clearer Initialised!");
        FogOfWar.Instance.AddClearer(this);
        if (currentRoom != null) {
            FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
            roomFound = true;
        }
    }

    protected override void ChangedRoom() {
        FogOfWar.Instance.OnFogClearerLeaveRoom(currentRoom);

        currentRoom = FindCurrentRoom();

        FogOfWar.Instance.OnFogClearerEnterRoom(currentRoom);
        Debug.Log(currentRoom);
    }

    public bool IsInRoom(Room room) {
        return room.Contains(transform.position.RoundToInt());
    }
}
