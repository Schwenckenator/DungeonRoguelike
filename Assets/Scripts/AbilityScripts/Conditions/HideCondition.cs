using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCondition : Condition {

    public float hiddenAlpha = 0.5f;

    protected override void StartCondition(int minValue, int maxValue) {
        base.StartCondition(minValue, maxValue);
        remainingLifetime = Random.Range(minValue, maxValue + 1);

        target.EntityVisibilityController.SetEntityAlpha(hiddenAlpha);
    }

    protected override void EndCondition() {
        base.EndCondition();
        target.EntityVisibilityController.SetEntityAlpha(1);
    }

}
