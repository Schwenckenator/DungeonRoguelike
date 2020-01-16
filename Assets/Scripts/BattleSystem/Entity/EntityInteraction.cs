using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class EntityInteraction : MonoBehaviour
{
    
    public List<Ability> abilities; //Set this in inspector

    public Collider2D selector;
    //public int raycount = 16;
    //public float rayDistance = 2f;

    private Entity myEntity;
    private Ability currentAbility;
    private ContactFilter2D contactFilter;
    
    private void OnEnable() {
        PlayerInput.Instance.onMouseHover += HoverOverTarget;
        PlayerInput.Instance.onLeftMouseButtonPressed += SelectTarget;
        PlayerInput.Instance.onRightMouseButtonPressed += CancelTargeting;
        //targetingRing.SetActive(true);
        selector.gameObject.SetActive(true);
    }
    private void OnDisable() {
        PlayerInput.Instance.onMouseHover -= HoverOverTarget;
        PlayerInput.Instance.onLeftMouseButtonPressed -= SelectTarget;
        PlayerInput.Instance.onRightMouseButtonPressed -= CancelTargeting;
        //targetingRing.SetActive(false);
        selector.gameObject.SetActive(false);
    }

    public void Initialise() {
        myEntity = GetComponent<Entity>();
        //Debug.Log(myEntity.character.baseAbilities.Count.ToString());
        //Select first ability for safety
        abilities = myEntity.character.baseAbilities;

        //currentAbility = abilities[0];
        SetCurrentAbility(0);
        contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        
    }

    private void Update() {
        if(myEntity.State != EntityState.targeting) {
            Debug.LogError("This should not run while not targeting. Aborting.");
            this.enabled = false;
            return;
        }
    }

    private void HoverOverTarget(Vector2 worldPoint) {
        //Vector2 movePoint = worldPoint;

        ////Raycast at position
        //RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        //foreach (var hit in hits) {

        //    if (hit.collider.CompareTag("Entity")) {
        //        movePoint = hit.collider.transform.position;
        //    }
        //}

        //selector.transform.position = movePoint;
        if (currentAbility.PositionLocked) {
            RotateSelector(worldPoint);
            MoveSelector(this.transform.position);
        } else {
            MoveSelector(worldPoint);
        }
    }
    private void MoveSelector(Vector2 worldPoint) {
        Vector2 movePoint = worldPoint;

        //Raycast at position
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        foreach (var hit in hits) {

            if (hit.collider.CompareTag("Entity")) {
                movePoint = hit.collider.transform.position;
            }
        }

        selector.transform.position = movePoint;
    }
    private void RotateSelector(Vector2 worldPoint) {
        //Find angle between Vector.Right and mouse point
        Vector2 relativePosition = worldPoint.ToVector3() - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, relativePosition);
        

        selector.transform.rotation = Quaternion.Euler(0, 0, angle);

        //Debug.Log($"Rotation! WP: {worldPoint}, RP: {relativePosition}, Ang: {angle}.");
        

        //TODO: Make rotation?
    }
    public void SelectTarget(Vector2 worldPoint) {

        if (!IsValidInteraction(worldPoint)) {
            return;
        }
        Debug.Log("Checking Collisions with targeting circle.");
        var hitsList = new List<Collider2D>();
        var targetList = new List<Entity>();

        Physics2D.OverlapCollider(selector, contactFilter, hitsList);

        foreach (var hit in hitsList) {
            Debug.Log($"{hit.ToString()} hit");
            if (hit.CompareTag("Entity")) {
                targetList.Add(hit.GetComponent<Entity>());
            }
        }

        int validTargets = 0;
        foreach (var target in targetList) {
            //Check for correct target
            if (!currentAbility.IsLegalTarget(myEntity, target)) {
                Debug.Log("Not Legal Target!");
                continue;
            }

            //Do the ability
            currentAbility.TriggerAbility(target);
            validTargets++;
        }
        if(!currentAbility.requireValidTarget || validTargets > 0) {
            currentAbility.DisplayVisual(worldPoint);
            SpendActions();
        }
    }
    private void CancelTargeting(Vector2 waste) {
        myEntity.State = EntityState.idle;
    }

    private void SpendActions() {
        int actionCost = currentAbility.actionCost;

        if (currentAbility.endsTurn) { // Spend all remaining actions
            actionCost = myEntity.TurnScheduler.actionsRemaining;
        }
        //Spend the actions
        myEntity.TurnScheduler.SpendActions(actionCost);
        Invoke("CheckForEndOfTurn", 1f);
        myEntity.State = EntityState.idle;
    }
    private bool IsValidInteraction(Vector3 worldPoint) {
        //First check action count
        if (myEntity.TurnScheduler.actionsRemaining < currentAbility.actionCost) {
            Debug.Log("Not enough Actions remaining!");
            return false;
        }

        //Check range to target
        if ((transform.position - worldPoint).magnitude > currentAbility.range + 0.9f) { //Add a lot of grace
            Debug.Log("Out of range!");
            return false;
        }
        Debug.Log("No obvious impediment.");
        return true;
    }

    private void CheckForEndOfTurn() {
        myEntity.TurnScheduler.ActionFinished();
    }

    public void SetCurrentAbility(int index) {
        currentAbility = abilities[index];

        GameObject obj = selector.gameObject;
        currentAbility.PrepareSelector(ref obj);
        //if(currentAbility is SingleTargetAbility) {
        //    SelectorObj = targetingRing;
        //}else if(currentAbility as CircleAreaAbility) {
        //    SelectorObj = areaSelector;
            
        //}
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