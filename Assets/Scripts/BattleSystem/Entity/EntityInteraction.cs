using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class EntityInteraction : MonoBehaviour
{
    
    public List<Ability> abilities; //Set this in inspector
    public GameObject targetingRing;
    public GameObject areaSelector;


    private GameObject SelectorObj {
        get
        {
            return selectorObj;
        }
        set
        {
            var oldObj = selectorObj;

            selectorObj = value;

            if (oldObj != null && oldObj.activeInHierarchy) {
                oldObj.SetActive(false);
                selectorObj.SetActive(true);
            }

            selector = value.GetComponent<Collider2D>();
        }
    }
    private GameObject selectorObj;
    private Collider2D selector;
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
        SelectorObj.SetActive(true);
    }
    private void OnDisable() {
        PlayerInput.Instance.onMouseHover -= HoverOverTarget;
        PlayerInput.Instance.onLeftMouseButtonPressed -= SelectTarget;
        PlayerInput.Instance.onRightMouseButtonPressed -= CancelTargeting;
        //targetingRing.SetActive(false);
        SelectorObj.SetActive(false);
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
        Vector2 movePoint = worldPoint;
        
        //Raycast at position
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);

        foreach (var hit in hits) {

            if (hit.collider.CompareTag("Entity")) {
                movePoint = hit.collider.transform.position;
            }
        }

        SelectorObj.transform.position = movePoint;
    }
    private void SelectTarget(Vector2 worldPoint) {

        var hits = Physics2D.OverlapCircleAll(worldPoint, 0.1f);
        var hitsList = new List<Collider2D>();

        Physics2D.OverlapCollider(selector, contactFilter, hitsList);
        //RaycastHit2D[] hits = Physics2D(worldPoint, Vector2.zero);

        foreach (var hit in hits) {
            if (hit.CompareTag("Entity")) {
                Interact(hit.GetComponent<Entity>());
            }
        }
    }

    private void CancelTargeting(Vector2 waste) {
        myEntity.State = EntityState.idle;
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
        SelectorObj = currentAbility.PrepareSelector();
        Debug.Log($"Is selector null. {SelectorObj == null}");
        //if(currentAbility is SingleTargetAbility) {
        //    SelectorObj = targetingRing;
        //}else if(currentAbility as CircleAreaAbility) {
        //    SelectorObj = areaSelector;
            
        //}
    }

    //private static Vector2 RotateVector(Vector2 input, float degrees) {
    //    Vector2 output = Vector2.zero;

    //    float theta = Mathf.Deg2Rad * degrees;

    //    float cos = Mathf.Cos(theta);
    //    float sin = Mathf.Sin(theta);

    //    float xprime = input.x * cos - input.y * sin;
    //    float yprime = input.x * sin + input.y * cos;

    //    output = new Vector2(xprime, yprime);

    //    Debug.Log($"Rotation! Input:{input.ToString()} is now {output.ToString()}");

    //    return output;
    //}
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