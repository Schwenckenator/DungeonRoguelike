using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    public GameObject hero;
    public GameObject monster;

    public void StartBattle() {
        BattleController.Instance.StartBattle();
    }

    public void SpawnHero() {
        Spawn(hero);
    }
    public void SpawnMonster() {
        Spawn(monster);
    }
    public void Spawn(GameObject obj) {
        //Get Random position
        Vector2 randomPosition = new Vector2(Random.Range(-3, 3), Random.Range(-3, 3));
        Instantiate(obj, randomPosition, Quaternion.identity);
    }
}
