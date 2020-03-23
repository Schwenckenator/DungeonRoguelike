using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInventory : MonoBehaviour
{
    private Entity myEntity;

    private List<Item> items;

    private List<WorldItem> itemsStoodOn;

    public void Initialise() {
        myEntity = GetComponent<Entity>();
        items = new List<Item>();
        itemsStoodOn = new List<WorldItem>();

        foreach(Item item in myEntity.character.startingItems) {
            AddItem(item);
        }
    }

    public void AddItem(Item item) {
        items.Add(item);
        foreach(Ability ability in item.abilities) {
            myEntity.Interaction.AddAbility(ability);
        }
        foreach(StatModifier mod in item.passives) {
            myEntity.Stats.Collection.AddModifier(mod);
        }
    }

    public void RemoveItem(Item item) {
        items.Remove(item);
        foreach(Ability ability in item.abilities) {
            myEntity.Interaction.RemoveAbility(ability);
        }
        foreach (StatModifier mod in item.passives) {
            myEntity.Stats.Collection.RemoveModifier(mod);
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

    public bool IsCollidingWithWorldItem() {
        return itemsStoodOn.Count > 0;
    }

}
