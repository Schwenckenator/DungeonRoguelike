﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityState { inactive, targeting, moving, idle}
public enum EntityAllegience { player, monster}

public class Entity : MonoBehaviour
{
    public EntityInteraction Interaction { get; private set; }
    public EntityStats Stats { get; private set; }
    public EntityTurnScheduler TurnScheduler { get; private set; }
    public ClickToMove ClickToMove { get; private set; }

    private EntityState state;
    public EntityState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
            if(state == EntityState.moving) {
                ClickToMove.enabled = true;
                Interaction.enabled = false;
                TargetingRing.Instance.SetEnabled(false);

            } else if(state == EntityState.idle) {

                Debug.Log("SET IDLE");
                ClickToMove.enabled = true;
                Interaction.enabled = false;
                TargetingRing.Instance.SetEnabled(false);

            } else if(state == EntityState.targeting) {
                ClickToMove.enabled = false;
                Interaction.enabled = true;
                TargetingRing.Instance.SetEnabled(true);

            } else if(state == EntityState.inactive) {
                ClickToMove.enabled = false;
                Interaction.enabled = false;
                
            }
        }
    }
    public EntityAllegience allegience;

    // Start is called before the first frame update
    void Start()
    {
        Interaction = GetComponent<EntityInteraction>();
        Stats = GetComponent<EntityStats>();
        TurnScheduler = GetComponent<EntityTurnScheduler>();
        ClickToMove = GetComponent<ClickToMove>();

        State = EntityState.inactive;
    }

}
