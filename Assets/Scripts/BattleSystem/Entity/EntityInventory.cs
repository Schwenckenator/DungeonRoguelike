using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityInventory : MonoBehaviour
{
    private Entity myEntity;

    private Dictionary<Item, int> items;
    
    private List<WorldItem> itemsStoodOn;

    public void Initialise() {
        myEntity = GetComponent<Entity>();
        items = new Dictionary<Item, int>();
        itemsStoodOn = new List<WorldItem>();

        foreach(Item item in myEntity.character.startingItems) {
            AddItem(item);
        }
    }

    public void AddItem(Item item) {
        int count = item.charges <= 0 ? 1 : item.charges; // Return either 1 or the number of charges

        if (items.ContainsKey(item)) {
            items[item] += count;
        } else {
            items.Add(item, count);
        }

        foreach(Ability ability in item.abilities) {
            ItemCallback callback = null;
            if (item.isConsumable) {
                callback = new ItemCallback(item, DecrementCharges);
            }
            myEntity.Interaction.AddAbility(ability, callback);
        }
        foreach(Buff buff in item.passives) {
            myEntity.Stats.AddBuff(buff);
        }
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
        foreach(Ability ability in item.abilities) {
            myEntity.Interaction.RemoveAbility(ability);
        }
        foreach (Buff buff in item.passives) {
            myEntity.Stats.RemoveBuff(buff);
        }
    }

    public void OnItemCollisionEnter(WorldItem item) {
        itemsStoodOn.Add(item);
    }
    public void OnItemCollisionExit(WorldItem item) {
        itemsStoodOn.Remove(item);
    }

    public void PickUpItem(WorldItem item) {
        AddItem(item.myItem);
        Destroy(item.gameObject);
    }

    public void PickUpItemsOnFloor() {
        for(int i=0; i<itemsStoodOn.Count; i++) {
            PickUpItem(itemsStoodOn[i]);
        }
        itemsStoodOn.Clear();
    }

    public void DropItem(Item item) {
        RemoveItem(item);
        ItemGenerator.Instance.SpawnItem(item, transform.position.RoundToVector2Int());
    }

    public void DropAllDroppable() {
        Debug.Log("Dropping droppables!");
        var droppables = new List<Item>();
        foreach(Item item in items.Keys) {
            if (item.isDroppable) {
                droppables.Add(item);
            }
        }
        foreach(Item item in droppables) {
            Debug.Log($"Dropping {item}");
            DropItem(item);
        }
    }

    public bool IsCollidingWithWorldItem() {
        return itemsStoodOn.Count > 0;
    }

    private void DecrementCharges(Item item) {
        Debug.Log("Charges decremented!");
        items[item]--;
        if (items[item] <= 0) {
            Debug.Log("Charges are 0, item removed!");
            RemoveItem(item);
        }
    }

}
