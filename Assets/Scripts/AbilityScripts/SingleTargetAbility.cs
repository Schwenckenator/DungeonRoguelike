using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Single Target Ability", order = 51)]
public class SingleTargetAbility : Ability {

    public override void TriggerAbility(Entity target) {
        if(abilityType == AbilityType.damage) {
            target.Stats.ModifyHealth((Random.Range(minValue, maxValue + 1)) * -1); //Add one because range excludes maximum value
        }else if(abilityType == AbilityType.heal) {
            target.Stats.ModifyHealth(Random.Range(minValue, maxValue + 1));
        }
    }

    //public override void Initialise() {
    //    throw new System.NotImplementedException();
    //}
}
