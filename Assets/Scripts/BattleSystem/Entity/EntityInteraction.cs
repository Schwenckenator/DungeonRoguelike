using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public enum InteractionType {
    attack, heal
}

public class EntityInteraction : MonoBehaviour
{
    public Entity myEntity;
    public Ability currentAbility;
    public List<Ability> abilities;

    public int raycount = 16;
    public float rayDistance = 2f;

    private void Start() {
        myEntity = GetComponent<Entity>();
        abilities = new List<Ability> {
            //Add two abilities
            new BasicAttack(TargetType.enemy, 2, 1f, 15, 30),
            new BasicAttack(TargetType.ally, 2, 1f, -15, -30)
        };

    }

    private void Update() {
        if(myEntity.State != EntityState.targeting) {
            Debug.LogError("This should not run while not targeting. Aborting.");
            this.enabled = false;
            return;
        }
        // If the pointer is over a UI element, the player doesn't want to set a target.
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //
        AcquireTarget();
    }

    private void AcquireTarget() {
        //Find mouse position;
        var mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint2d, Vector2.zero);

        foreach (RaycastHit2D hit in hits) {

            if (hit.collider.CompareTag("Entity")) {
                //It has found an entity
                //Now display targeting ring, chance to hit etc.
                Debug.Log($"Mouse is over {hit.collider.gameObject.name}.");

                //If the player wants to select the target, they click
                if (Input.GetMouseButtonDown(0)) {
                    Interact(hit.collider.GetComponent<Entity>());
                }
            }
        }
    }

    public void Interact(Entity target) {

        currentAbility.Activate(target);
    }

    public void SetCurrentAbility(int index) {
        currentAbility = abilities[index];
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

        //EntityInteraction myScript = (EntityInteraction)target;
        //if (Application.isPlaying) {
        //    if (GUILayout.Button("Aquire targets all around")) {
        //        myScript.AllAroundTargeting();
        //    }
            //if (GUILayout.Button("Attack!")) {
            //    myScript.Interact(InteractionType.attack);
            //}
            //if (GUILayout.Button("Heal")) {
            //    myScript.Interact(InteractionType.heal);
            //}
        //}
    }
}