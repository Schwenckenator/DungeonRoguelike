using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCondition : Condition {

    public float hiddenAlpha = 0.5f;

    protected override void StartCondition(int minValue, int maxValue) {

        remainingLifetime = Random.Range(minValue, maxValue + 1);

        target.EntityVisibilityController.SetEntityAlpha(hiddenAlpha);
        target.Stats.AddCondition(ConditionType.hidden);
    }

    protected override void EndCondition() {
        target.EntityVisibilityController.SetEntityAlpha(1);
        target.Stats.RemoveCondition(ConditionType.hidden);
    }

}
