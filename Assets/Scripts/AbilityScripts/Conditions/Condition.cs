using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    public bool hasLifetime = true;
    public int lifetime;
    protected int remainingLifetime;

    protected Entity target;

    public void Apply(Entity entity, int minValue, int maxValue) {
        target = entity;

        if (hasLifetime) {
            remainingLifetime = lifetime;
            GameEvents.current.onStartPlayerTurn += DecrementLifetime;
        }
        StartCondition(minValue, maxValue);
    }

    protected abstract void StartCondition(int minValue, int maxValue);
    protected abstract void EndCondition();

    protected virtual void DecrementLifetime(int entityID) {
        // Check only get responding to relevant entity
        if (entityID == target.gameObject.GetInstanceID()) {
            remainingLifetime--;
            if (remainingLifetime <= 0) {
                EndCondition();
                GameEvents.current.onStartPlayerTurn -= DecrementLifetime;
                Destroy(this);
            }
        }
    }
}
