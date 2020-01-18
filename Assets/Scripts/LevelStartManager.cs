using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartManager : MonoBehaviour
{
    public bool generateOnStart = true;

    private bool isGenerating = false;

    public Dungeon dungeon;
    public EncounterGenerator encounterGenerator;
    public HeroSpawner heroSpawner;

    // Start is called before the first frame update
    void Start()
    {
        if (generateOnStart) {
            AttemptToGenerateLevel();
        }
    }

    public void AttemptToGenerateLevel() {
        if (isGenerating) {
            Debug.LogWarning("Level is already generating, aborting...");
            return;
        }
        StartCoroutine(GenerateLevel());
    }

    private IEnumerator GenerateLevel() {
        //Pause for 1 frame
        yield return null;

        //Generate Dungeon
        yield return StartCoroutine(dungeon.GenerateDungeon());
        //Generate Encounters
        yield return StartCoroutine(encounterGenerator.GenerateEncounters(dungeon));
        //Generate Heroes
        heroSpawner.SpawnHeroes(dungeon.rooms[0]); // first room
        //Generate Items
        ItemGenerator.Instance.GenerateItems(dungeon);
        //Start Battle
        BattleController.Instance.StartBattle();
    }

}
