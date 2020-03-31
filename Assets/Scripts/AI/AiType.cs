using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ai Type", menuName = "AI/New AiType", order = 53)]
public class AiType : ScriptableObject
{
    public string aiTypeName;
    //Eg front line would like to be at 1 distance when choosing attacks
    public float preferredFightRange;
    public float patrolRadius;
    public float chaseRange;
    public float endChaseRange;
    public bool debug = true;



    public void makeDecision(Entity me, Entity nearestEntity)
    {
        float distanceToEntity = (nearestEntity.transform.position - me.transform.position).magnitude;

        bool nearestEntityExists = false;

        if(nearestEntity!= null)
        {
            nearestEntityExists = true;
        }


        if (nearestEntityExists && distanceToEntity <= preferredFightRange)
        {
            ShouldAttack(me, nearestEntity);
        }
        else if(nearestEntityExists && distanceToEntity <= chaseRange && distanceToEntity < endChaseRange)
        {
            ShouldChase(me, nearestEntity);

        }
        else if (patrolRadius > 0)
        {
            ShouldPatrol(me);

        }
        else
        {
            ShouldDoNothing(me);
        }
    }

    public void ShouldChase(Entity me, Entity nearestEntity)
    {
        me.AiController.MoveToNearestPlayer(nearestEntity);
    }

    public void ShouldAttack(Entity me, Entity nearestEntity)
    {
        me.AiController.Attack(nearestEntity);
    }
    public void ShouldPatrol(Entity me)
    {
        //TODO call AI Controller
    }
    public void ShouldDoNothing(Entity me) {
        //Added just for completeness, might need to know down the track
    }
}
