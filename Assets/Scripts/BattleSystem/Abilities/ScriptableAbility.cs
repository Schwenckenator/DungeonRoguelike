using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ScriptableAbility", menuName = "ScriptableObjects/Ability", order = 51)]
public class ScriptableAbility : ScriptableObject
{
    [SerializeField]
    private string name;

    public int actionCost;
}
