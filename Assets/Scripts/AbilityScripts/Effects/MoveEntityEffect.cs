using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MoveEntityEffect", menuName = "Effect/Move Entity Effect", order = 52)]
public class MoveEntityEffect : Effect {
    public enum MoveType { push, pull } // slide (not implemented


    public MoveType moveType;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="minValue">Min number of squares moved</param>
    /// <param name="maxValue">Max number of squares moved</param>
    public override void TriggerEffect(Entity origin, Entity target, int minValue, int maxValue) {
        Debug.Log("Trigger move effect");
        // Find direction of forced movement
        Vector2 dir;
        if (moveType == MoveType.push) {
            dir = (target.transform.position - origin.transform.position).normalized;
        } else if (moveType == MoveType.pull) {
            dir = (origin.transform.position - target.transform.position).normalized;
        } else {
            throw new System.NotImplementedException("Slide not implemented");
        }

        // Check for obstructions
        RaycastHit2D[] hits = Physics2D.RaycastAll(target.transform.position, dir, maxValue, LayerMask.GetMask("Entity", "Obstacle"));

        float maxDistance = maxValue;
        Debug.Log($"Starting max distance is {maxDistance}");
        foreach(var hit in hits) {
            if(hit.collider.gameObject == target.gameObject) {
                //It's the target colliding with itself, skip
                Debug.Log("Hit target's collider, ignoring");
                continue;
            }
            
            maxDistance = Mathf.Min(hit.distance, maxDistance);
            Debug.Log($"Hit {hit.collider}, New max distance is {maxDistance}");
        }

        int moveDistance = Random.Range(minValue, Mathf.FloorToInt(maxDistance) + 1);

        Debug.Log($"Move distance = {moveDistance}");

        Vector2Int goal = target.transform.position.RoundToVector2Int() + (dir * moveDistance).RoundToInt();
        Debug.Log($"Goal position is {goal}");

        target.PathAgent.ForceMove(goal);
    }
}
