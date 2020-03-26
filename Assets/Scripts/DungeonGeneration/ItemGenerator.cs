using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public static ItemGenerator Instance { get; private set; }

    public GameObject itemBase;
    public RangeInt numberOfItems;
    public List<Item> itemList;

    private void Start() {
        Instance = this;
    }

    public void GenerateItems(Dungeon dungeon) {
        for (int i = 0; i < numberOfItems.GetRandom(); i++) {
            //Pick a random room, generate encounter, then remove it from the list
            Room room = dungeon.rooms.RandomItem();
            SpawnRandomItem(room);
        }
    }

    public void SpawnRandomItem(Room room) {
        Vector2Int position = room.RandomSpawnablePoint();
        Item item = itemList.RandomItem();
        SpawnItem(item, position);
    }

    public void SpawnItem(Item item, Vector2Int position) {
        GameObject newWorldItem = Instantiate(itemBase, position.ToVector3Int(), Quaternion.identity);
        newWorldItem.GetComponent<WorldItem>().Initialise(item);
    }
}
