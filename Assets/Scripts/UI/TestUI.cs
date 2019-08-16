using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    public GameObject hero;
    public GameObject monster;
    public Character warrior;
    public Character thief;
    public Character cleric;
    public Character goblin;

    public void StartBattle() {
        BattleController.Instance.StartBattle();
    }

    public void EndBattle() {
        BattleController.Instance.EndBattle();
    }

    public void SpawnHero(string heroType) {
        if(heroType == "warrior") Spawn(hero, warrior);
        else if(heroType == "thief") Spawn(hero, thief);
        else if (heroType == "cleric") Spawn(hero, cleric);
    }
    public void SpawnMonster(string monsterType) {
        if(monsterType == "goblin") Spawn(monster, goblin);
    }
    public void Spawn(GameObject obj, Character character) {
        //Get Random position
        Vector2 randomPosition = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
        GameObject newEntity = Instantiate(obj, randomPosition, Quaternion.identity);
        newEntity.GetComponent<Entity>().character = character;
        newEntity.GetComponent<Entity>().Initialise();
    }
}
