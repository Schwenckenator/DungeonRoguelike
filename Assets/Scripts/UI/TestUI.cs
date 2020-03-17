using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    public Dungeon dungeon;
    public GameObject hero;
    public GameObject monster;
    public Character warrior;
    public Character thief;
    public Character cleric;
    public Character goblin;
    public Character archer;

    private void Start() {
        dungeon = FindObjectOfType<Dungeon>();
    }

    public void StartBattle() {
        BattleController.Instance.StartBattle();
    }

    public void EndBattle() {
        BattleController.Instance.EndBattle();
    }

    public void SpawnUnits(int number) {
        Debug.Log($"Spawning {number} warriors.");
        for(int i=0; i < number; ++i) {
            //Spawn(hero, warrior);
            Spawn(monster, goblin);
        }
    }

    public void SpawnHero(string heroType) {
        if(heroType == "warrior") Spawn(hero, warrior);
        else if(heroType == "thief") Spawn(hero, thief);
        else if (heroType == "cleric") Spawn(hero, cleric);
        else if (heroType == "archer") Spawn(hero, archer);

    }
    public void SpawnMonster(string monsterType) {
        if(monsterType == "goblin") Spawn(monster, goblin);
    }
    public void Spawn(GameObject obj, Character character) {
        //Get Random position
        Vector2 randomPosition = dungeon.RandomSpawnablePosition();

        GameObject newEntity = Instantiate(obj, randomPosition, Quaternion.identity);
        newEntity.GetComponent<Entity>().character = character;
        newEntity.GetComponent<Entity>().Initialise();
    }
}
