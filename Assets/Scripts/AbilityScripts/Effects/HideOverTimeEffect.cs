using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HideOverTimeEffect", menuName = "Effect/Hide OverTime Effect", order = 52)]
public class HideOverTimeEffect : OTEffect {
    public int remainingActiveTurns;
    public float hiddenAlpha = 0.5f;
    // Overtime Effect
    public OTEffect oTEffect;

    public override void TriggerEffect(Entity target, int minValue, int maxValue) {
        target.Stats.AddOvertimeEffect(oTEffect);

    }
    private void EnterHide(Entity me)
    {
        me.EntityVisibilityController.SetEntityAlpha(hiddenAlpha);
    }
    public void ExitHide(Entity me)
    {
        me.EntityVisibilityController.SetEntityAlpha(1);

    }
    private void Update()
    {

    }

    public override void ActivateTriggerEffect(Entity target, int minValue, int maxValue)
    {
        EnterHide(target);
    }

    public override void DeactivateTriggerEffect(Entity target, int minValue, int maxValue)
    {
        throw new System.NotImplementedException();
    }
}
