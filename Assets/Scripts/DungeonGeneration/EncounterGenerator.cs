using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script receives a completed dungeon as input, and places appropriate encounters in rooms as output.
/// </summary>
public class EncounterGenerator : MonoBehaviour
{

    public static EncounterGenerator Instance { get; private set; }

    public int numOfEncounters;
    public int monstersPerEncounter;
    public Character monster;

    private void Start() {
        Instance = this;
    }

    public void GenerateEncounters(Dungeon dungeon) {
        List<Room> freeRooms = dungeon.rooms;

        for(int i=0; i<numOfEncounters; i++) {
            //Pick a random room, generate encounter, then remove it from the list
            Room room = freeRooms.RandomItem();
            GenerateSingleEncounter(room);
            freeRooms.Remove(room);
        }
    }

    private void GenerateSingleEncounter(Room room) {
        //foreach member of an encounter
        for (int i=0; i<monstersPerEncounter; i++) {
            EntitySpawner.Instance.SpawnEntity(monster, room);
        }
    }
}
