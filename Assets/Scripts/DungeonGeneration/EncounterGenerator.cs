using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Monster
{
    public Character character;
    [Header("The total ratio of all monsters should add up to 1")]
    public float ratio;
    //Was thinking this for when we need a boss spawn
    [Header("Leave this at 0 for no max")]
    public int maxSpawns;

    private int amountSpawned;

    private bool Spawnable()
    {
        if(maxSpawns>0 && amountSpawned <= maxSpawns) return true;
        return false;
    }
    public void Spawn()
    {

        maxSpawns += 1;
    }
    public int Spawned()
    {
        return amountSpawned;
    }
}

/// <summary>
/// This script receives a completed dungeon as input, and places appropriate encounters in rooms as output.
/// </summary>
public class EncounterGenerator : MonoBehaviour
{

    public static EncounterGenerator Instance { get; private set; }

    //public int numOfEncounters;
    //public int monstersPerEncounter;
    public RangeInt numOfEncounters;
    public RangeInt monstersPerEncounter;
    //public Character monster;
    public List<Monster> monsters;

    private void Start() {
        Instance = this;
    }

    public IEnumerator GenerateEncounters(Dungeon dungeon) {
        List<Room> freeRooms = new List<Room>(dungeon.rooms);

        freeRooms.RemoveAt(0); // Remove the first room, that's where the heroes spawn

        for(int i=0; i<numOfEncounters.GetRandom(); i++) {
            //Pick a random room, generate encounter, then remove it from the list
            Room room = freeRooms.RandomItem();
            GenerateSingleEncounter(room);
            freeRooms.Remove(room);
            yield return null;
        }
    }

    private void GenerateSingleEncounter(Room room) {
        //foreach member of an encounter
        int totalMonstersPerEncounter = monstersPerEncounter.GetRandom();

        foreach(Monster monster in monsters)
        {
            int amountToSpawn = (int)Math.Round(monster.ratio * totalMonstersPerEncounter);
            for (int i=0; i< amountToSpawn; i++) {
                EntitySpawner.Instance.SpawnEntity(monster.character, room);
            }
        }

    }
    
}
