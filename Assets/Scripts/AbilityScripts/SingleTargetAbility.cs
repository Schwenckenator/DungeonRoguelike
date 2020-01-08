using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SingleTargetAbility", menuName = "Ability/Single Target Ability", order = 51)]
public class SingleTargetAbility : Ability {

    public override GameObject PrepareSelector() {
        return selector;
    }

    //public override void Initialise() {
    //    throw new System.NotImplementedException();
    //}
}
