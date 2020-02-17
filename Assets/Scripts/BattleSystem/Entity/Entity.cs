using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridPathfinding;

public enum EntityState { inactive, targeting, moving, idle}
public enum EntityAllegiance { player, monster}

public class Entity : MonoBehaviour
{
    public Character character;

    public EntityInteraction Interaction { get; private set; }
    public EntityStats Stats { get; private set; }
    public EntityTurnScheduler TurnScheduler { get; private set; }
    public PathAgent PathAgent { get; private set; }
    //public ClickToMove ClickToMove { get; private set; }
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
                PathAgent.enabled = true;
                Interaction.enabled = false;
                

            } else if(state == EntityState.idle) {

                //Debug.Log("SET IDLE");
                PathAgent.enabled = true;
                Interaction.enabled = false;
                

            } else if(state == EntityState.targeting) {
                PathAgent.enabled = false;
                Interaction.enabled = true;
                

            } else if(state == EntityState.inactive) {
                PathAgent.enabled = false;
                Interaction.enabled = false;
                
            }
        }
    }

    private SpriteRenderer spriteRenderer;


    public void Initialise()
    {
        if(character == null) {
            Debug.LogError("Character cannot be null!");
            return;
        }
        Interaction = GetComponent<EntityInteraction>();
        Stats = GetComponent<EntityStats>();
        TurnScheduler = GetComponent<EntityTurnScheduler>();
        PathAgent = GetComponent<PathAgent>();

        State = EntityState.inactive;

        Interaction.Initialise();
        Stats.Initialise();
        TurnScheduler.Initialise();
        PathAgent.Initialise();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = character.sprite;
    }

    //TODO: Graphics should be moved out of this script
    public void Die() {
        spriteRenderer.sprite = character.deadSprite;
    }
}
