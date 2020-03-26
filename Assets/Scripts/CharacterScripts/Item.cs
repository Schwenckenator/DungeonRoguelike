using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item", order = 55)]
public class Item : ScriptableObject
{
    public new string name;
    public bool isConsumable;
    public int charges;
    public bool isDroppable;
    public Ability[] abilities;
    public Buff[] passives;
    public Sprite sprite;
}
