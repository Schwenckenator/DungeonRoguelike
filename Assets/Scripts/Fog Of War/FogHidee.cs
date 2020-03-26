using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHidee : FogInteractor
{
    //private Dungeon dungeon;
    //private Room currentRoom;
    //private bool roomFound = false;
    public Renderer[] renderers;
    public GameObject ui;

    public override void Initialise() {
        base.Initialise();
        Debug.Log($"Fog Hidee Initialised!");
        FogOfWar.Instance.AddHidee(this);
    }

    public void SetVisible(bool visible) {
        Debug.Log($"Fog Hidee Set Visible: {visible}");
        foreach(var renderer in renderers) {
            renderer.enabled = visible;
        }
        ui.SetActive(visible);
    }

    protected override void ChangedRoom() {
        currentRoom = FindCurrentRoom();

        if (FogOfWar.Instance.RoomOccupiedByClearer(currentRoom)) {
            SetVisible(true);
        }
    }
}
