using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public Entity MyEntity { get; private set; }


    //Basic AI

    //Checks all enemies
    //Finds nearest enemy
    //Moves next to enemy
    //Attacks

    //If no enemies found, ends turn

    private Entity currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        MyEntity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartTurn() {

        Entity nearestEntity = GetNearestEntity(FindTargets());

        float distanceToEntity = (nearestEntity.transform.position - transform.position).magnitude;

        //If out of punching range
        if (distanceToEntity > 1f) {
            //Move towards target

        } else {
            //You're in range! Punch the sucker!

        }

    }

    private static List<Entity> FindTargets() {
        GameObject[] entities = GameObject.FindGameObjectsWithTag("Entity");
        List<Entity> possibleTargets = new List<Entity>();

        foreach (var obj in entities) {
            Entity entity = obj.GetComponent<Entity>();
            if (entity.allegiance != EntityAllegiance.monster) {
                possibleTargets.Add(entity);
            }
        }

        return possibleTargets;
    }

    private Entity GetNearestEntity(List<Entity> targets) {
        Entity nearestEntity = null;
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        float currentSmallestSqrMagnitude = Mathf.Infinity;

        foreach (var entity in targets) {
            Vector2 entity2dPos = new Vector2(entity.transform.position.x, entity.transform.position.y);
            Vector2 distanceVector = (entity2dPos - myPos);
            if (distanceVector.sqrMagnitude < currentSmallestSqrMagnitude) {
                currentSmallestSqrMagnitude = distanceVector.sqrMagnitude;
                nearestEntity = entity;
            }
        }
        return nearestEntity;
    }
}
