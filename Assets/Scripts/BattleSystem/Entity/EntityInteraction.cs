using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class EntityInteraction : MonoBehaviour
{
    
    public List<Ability> abilities; //Set this in inspector

    //public int raycount = 16;
    //public float rayDistance = 2f;

    private Entity myEntity;
    private Ability currentAbility;

    private Camera mainCamera;


    private void Start() {
        myEntity = GetComponent<Entity>();
        mainCamera = Camera.main;
    }

    private void Update() {
        if(myEntity.State != EntityState.targeting) {
            Debug.LogError("This should not run while not targeting. Aborting.");
            this.enabled = false;
            return;
        }
        // If the pointer is over a UI element, the player doesn't want to set a target.
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(1)) { //Right click cancels
            myEntity.State = EntityState.idle;
        }
        //
        AcquireTarget();
    }

    private void AcquireTarget() {
        //Find mouse position;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint2d, Vector2.zero);

        foreach (RaycastHit2D hit in hits) {

            if (hit.collider.CompareTag("Entity")) {
                //It has found an entity
                //Now display targeting ring, chance to hit etc.
                //Debug.Log($"Mouse is over {hit.collider.gameObject.name}.");

                //If the player wants to select the target, they click
                if (Input.GetMouseButtonDown(0)) {
                    Interact(hit.collider.GetComponent<Entity>());
                }
            }
        }
    }

    public void Interact(Entity target) {
        //Check if actions are available
        if(myEntity.TurnScheduler.actionsRemaining < currentAbility.actionCost) {
            Debug.Log("Not enough Actions remaining!");
            return;
        }

        //Check for correct target
        if(!currentAbility.IsLegalTarget(myEntity, target)) {
            Debug.Log("Not Legal Target!");
            return;
        }

        //Check range to target
        if ((transform.position - target.transform.position).magnitude > currentAbility.range + 0.9f) { //Add a lot of grace
            Debug.Log("Out of range!");
            return;
        }

        

        //Do the ability
        currentAbility.TriggerAbility(target);

        int actionCost = currentAbility.actionCost;

        if (currentAbility.endsTurn) { // Spend all remaining actions
            actionCost = myEntity.TurnScheduler.actionsRemaining;
        }
        //Spend the actions
        myEntity.TurnScheduler.SpendActions(actionCost);
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