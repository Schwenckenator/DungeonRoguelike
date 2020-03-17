using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartManager : MonoBehaviour
{
    public bool generateDungeon = true;
    public bool spawnEncounters = true;
    public bool spawnHeroes = true;
    public bool spawnItems = false;
    public bool startBattle = true;
    public bool fogOfWar = true;


    private bool isGenerating = false;

    public Dungeon dungeon;
    public EncounterGenerator encounterGenerator;
    public HeroSpawner heroSpawner;
    public ItemGenerator itemGenerator;

    // Start is called before the first frame update
    void Start() 
    {
        if (generateDungeon) {
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

        if (fogOfWar) {
            FogOfWar.Instance.Initialise(dungeon);
        }

        yield return null;

        //Generate Heroes
        if (spawnHeroes) {
            heroSpawner.SpawnHeroes(dungeon.rooms[0]); // first room
        }

        yield return null;

        //Generate Encounters
        if (spawnEncounters) {
            yield return StartCoroutine(encounterGenerator.GenerateEncounters(dungeon));
        }

        yield return null;

        //Generate Items
        if (spawnItems) {
            itemGenerator.GenerateItems(dungeon);
        }

        yield return null;

        //Start Battle
        if (startBattle) {
            BattleController.Instance.StartBattle();
        }

    }

}
