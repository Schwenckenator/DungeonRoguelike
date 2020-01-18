﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInventory : MonoBehaviour
{
    private Entity myEntity;

    public List<Item> items;

    public void Initialise() {
        myEntity = GetComponent<Entity>();
    }

    public void AddItem(Item item) {
        foreach(Ability ability in item.abilities) {
            myEntity.Interaction.AddAbility(ability);
        }
    }

    public void RemoveItem(Item item) {
        foreach(Ability ability in item.abilities) {
            myEntity.Interaction.RemoveAbility(ability);
        }
    }

}