using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ApplyConditionEffect", menuName = "Effect/Apply Condition Effect", order = 52)]
public class ApplyConditionEffect : Effect {

    public GameObject conditionObject;

    public override void TriggerEffect(Entity origin, Entity target, int minValue, int maxValue) {
        GameObject newObject = Instantiate(conditionObject, target.transform);
        target.Stats.AddOvertimeEffect(newObject);
        newObject.GetComponent<Condition>().Apply(target, minValue, maxValue);
    }


}
