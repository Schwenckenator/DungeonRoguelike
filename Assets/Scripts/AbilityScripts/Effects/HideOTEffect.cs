using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HideOverTimeEffect", menuName = "Effect/Hide OverTime Effect", order = 52)]
public class HideOTEffect : Effect {
    public int remainingActiveTurns;
    public float hiddenAlpha = 0.5f;
    public bool active;
    // Overtime Effect

    public override void TriggerEffect(Entity target, int minValue, int maxValue) {
        //target.Stats.AddOvertimeEffect(this);
        EnterHide(target);
    }
    private void EnterHide(Entity me)
    {
        me.EntityVisibilityController.SetEntityAlpha(hiddenAlpha);
        active = true;
    }
    public void ExitHide(Entity me)
    {
        me.EntityVisibilityController.SetEntityAlpha(1);
        active = false;
    }
    private void Update()
    {

    }

}
