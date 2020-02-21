using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item", order = 55)]
public class Item : ScriptableObject
{
    public new string name;
    public Ability[] abilities;
    public StatModifier[] passives; // TODO: change to proper class

    public Sprite sprite;
}
