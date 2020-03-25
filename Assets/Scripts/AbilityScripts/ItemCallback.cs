using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemCallback
{
    public Action<Item> callback;
    public Item item;

    public ItemCallback(Item item, Action<Item> callback) {
        this.callback = callback;
        this.item = item;
    }

    public void Invoke() {
        callback?.Invoke(item);
    }
}
