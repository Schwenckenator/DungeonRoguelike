using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HideOverTimeEffect", menuName = "Effect/Hide OverTime Effect", order = 52)]
public class HideOTEffect : Effect {
    public int remainingActiveTurns;
    public float hiddenAlpha = 0.5f;
    public bool active;

    private Entity entity;
    // Overtime Effect


    public void OnEnable()
    {
   

    }

    public void OnDisable()
    {
        //Subscribe to player turn to decrement hide lifetime
        GameEvents.current.onStartPlayerTurn -= DecrementEffectLifetime;
    }


    public override void TriggerEffect(Entity target, int minValue, int maxValue) {
        // If first time subscribe and set entity parent
        if (!entity)
        {
            //Subscribe to player turn to decrement hide lifetime
            GameEvents.current.onStartPlayerTurn += DecrementEffectLifetime;
            entity = target;
        }
        EnterHide(target);
    }
    private void EnterHide(Entity me)
    {
        me.EntityVisibilityController.SetEntityAlpha(hiddenAlpha);
        active = true;
    }
    public void ExitHide()
    {
        entity.EntityVisibilityController.SetEntityAlpha(1);
        active = false;
    }

    private void DecrementEffectLifetime(int entityID)
    {
        // Check only get responding to relevant entity
        if (entityID == GetInstanceID())
        { 
            remainingActiveTurns -= 1;
            if (remainingActiveTurns <= 0)
            {
                ExitHide();
            }
        }
    }
    private void Update()
    {

    }

}
