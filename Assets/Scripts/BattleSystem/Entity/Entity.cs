﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityState { inactive, targeting, moving, idle}
public enum EntityAllegiance { player, monster}

public class Entity : MonoBehaviour
{
    public Character character;

    public EntityInteraction Interaction { get; private set; }
    public EntityStats Stats { get; private set; }
    public EntityTurnScheduler TurnScheduler { get; private set; }
    public ClickToMove ClickToMove { get; private set; }
    public EntityAllegiance allegiance;

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
                

            } else if(state == EntityState.idle) {

                //Debug.Log("SET IDLE");
                ClickToMove.enabled = true;
                Interaction.enabled = false;
                

            } else if(state == EntityState.targeting) {
                ClickToMove.enabled = false;
                Interaction.enabled = true;
                

            } else if(state == EntityState.inactive) {
                ClickToMove.enabled = false;
                Interaction.enabled = false;
                
            }
        }
    }
    

    
    public void Initialise()
    {
        if(character == null) {
            Debug.LogError("Character cannot be null!");
            return;
        }
        Interaction = GetComponent<EntityInteraction>();
        Stats = GetComponent<EntityStats>();
        TurnScheduler = GetComponent<EntityTurnScheduler>();
        ClickToMove = GetComponent<ClickToMove>();

        State = EntityState.inactive;

        Interaction.Initialise();
        TurnScheduler.Initialise();
        ClickToMove.Initialise();

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = character.sprite;
    }
}
