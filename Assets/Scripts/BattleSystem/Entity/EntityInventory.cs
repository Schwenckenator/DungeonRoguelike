using System.Collections;
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
        
    }

}
