using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum InteractionType {
    attack, heal
}

public class EntityInteraction : MonoBehaviour
{
    public EntityStats target;
    public float myDamage;
    public float myHealing;

    public int raycount = 16;
    public float rayDistance = 2f;


    public void SetTarget(EntityStats target) {
        this.target = target;
    }

    public void Interact(InteractionType interaction) {
        if(interaction == InteractionType.attack) {
            target.Damage(myDamage);
        } else if (interaction == InteractionType.heal) {
            target.Heal(myHealing);
        }
    }

    public void AllAroundTargeting() {
        Debug.Log("Attempting to acquire Targets!");
        target = null;
        for(int i=0; i< raycount; i++) {
            //Declare all variables
            RaycastHit2D[] hits;
            float degrees = i * (360f / raycount);
            Vector2 dir = RotateVector(Vector2.right, degrees);

            //Draw debug ray

            Debug.DrawRay(transform.position, dir * rayDistance, Color.green, 3f);

            hits = Physics2D.RaycastAll(transform.position, dir, rayDistance);
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider != null && hit.collider.tag == "Entity") {
                    target = hit.transform.GetComponent<EntityStats>();
                    break;
                }
            }
            if(target != null) {
                break;
            }
        }

    }

    private static Vector2 RotateVector(Vector2 input, float degrees) {
        Vector2 output = Vector2.zero;

        float theta = Mathf.Deg2Rad * degrees;

        float cos = Mathf.Cos(theta);
        float sin = Mathf.Sin(theta);

        float xprime = input.x * cos - input.y * sin;
        float yprime = input.x * sin + input.y * cos;

        output = new Vector2(xprime, yprime);

        Debug.Log($"Rotation! Input:{input.ToString()} is now {output.ToString()}");

        return output;
    }
}

[CustomEditor(typeof(EntityInteraction))]
public class EntityInteractionEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        EntityInteraction myScript = (EntityInteraction)target;
        if (Application.isPlaying) {
            if (GUILayout.Button("Aquire targets all around")) {
                myScript.AllAroundTargeting();
            }
            if (GUILayout.Button("Attack!")) {
                myScript.Interact(InteractionType.attack);
            }
            if (GUILayout.Button("Heal")) {
                myScript.Interact(InteractionType.heal);
            }
        }
    }
}