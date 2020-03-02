using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Monster, Hero}
/// <summary>
/// A class with public facing methods to spawn entities
/// </summary>
public class EntitySpawner : MonoBehaviour
{
    public static EntitySpawner Instance { get; private set; }
    public GameObject monsterBase;
    public GameObject heroBase;

    public void Start() {
        Instance = this;
    }

    public void SpawnEntity(Character character, Vector2Int position, EntityType entityType = EntityType.Monster) {
        GameObject obj;
        if(entityType == EntityType.Monster) {
            obj = monsterBase;
        } else {
            obj = heroBase;
        }

        GameObject newEntity = Instantiate(obj, position.ToVector3Int(), Quaternion.identity);

        newEntity.GetComponent<Entity>().character = character;
        newEntity.name = $"{entityType} Entity ({character.name})";
        newEntity.GetComponent<Entity>().Initialise();
    }

    //Spawn into a room.
    public void SpawnEntity(Character character, Room room, EntityType entityType = EntityType.Monster) {
        // Pick a random position in the room
        Vector2Int position = room.RandomSpawnablePoint();

        // Spawn at point
        SpawnEntity(character, position, entityType);
    }
}
