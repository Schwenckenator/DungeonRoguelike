using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSpawner : MonoBehaviour
{
    public static HeroSpawner Instance { get; private set; }
    
    public List<Character> heroesToSpawn;

    private void Start() {
        Instance = this;
    }

    public void SpawnHeroes(Room room) {
        foreach(Character hero in heroesToSpawn) {
            EntitySpawner.Instance.SpawnEntity(hero, room, EntityType.Hero);
        }
    }
}
