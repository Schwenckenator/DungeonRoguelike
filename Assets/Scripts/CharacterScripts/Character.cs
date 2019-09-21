using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/New Character", order = 52)]
public class Character : ScriptableObject
{
    public string charName;
    public Sprite sprite;
    public Sprite deadSprite;
    public List<Ability> baseAbilities;

}
