using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/New Character", order = 53)]
public class Character : ScriptableObject
{
    public string charName;
    public Sprite sprite;
    public Sprite deadSprite;
    public List<Ability> baseAbilities;

    public int vitality;
    public int might;
    public int grace;

    public Dictionary<string, int> GetAttributes() {
        var attributes = new Dictionary<string, int> {
            { "vitality_base", vitality },
            { "vitality_mod", vitality },
            { "might_base", might },
            { "might_mod", might },
            { "grace_base", grace },
            { "grace_mod", grace },
            { "hp_max", vitality * 10 },    // TODO: Fix this sick filth magic number
            { "hp_now",  vitality * 10}
        };

        return attributes;
    }

}
