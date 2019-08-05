using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    public int TickDelay { get; private set; }
    public int Tick { get; private set; }
    public int Entity { get; private set; } //TODO: Make this the entity class when it's made

    public Turn(int entity, int tickDelay) {
        Entity = entity;
        TickDelay = tickDelay;
    }

    public void SetTick(int currentTick) {
        Tick = currentTick + TickDelay;
    }

    public override string ToString() {
        return $"Turn: Entity {Entity}, Tick {Tick}.";
    }
}
