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
            SpawnItem(room);
        }
    }

    private void SpawnItem(Room room) {
        Vector2Int position = room.RandomSpawnablePoint();

        GameObject newItem = Instantiate(itemBase, position.ToVector3Int(), Quaternion.identity);
        newItem.GetComponent<WorldItem>().Initialise(itemList.RandomItem());
    }
}
