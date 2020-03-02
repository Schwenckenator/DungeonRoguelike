using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    public bool debug = true;

    public static PlayerInput Instance { get; private set; }
    public bool playerHasControl = true;

    public Action<Vector2> onLeftMouseButtonPressed;
    public Action<Vector2> onRightMouseButtonPressed;
    public Action<Transform> onSpaceButtonPressed;

    public Action<Vector2> onMouseHover;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
    }

    private void Update() {
        if (!playerHasControl) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition = new Vector2(worldPoint.x, worldPoint.y);


        if (Input.GetMouseButtonDown(0)) {
            onLeftMouseButtonPressed?.Invoke(mousePosition);
        }
        if (Input.GetMouseButtonDown(1)) {
            onRightMouseButtonPressed?.Invoke(mousePosition);
        }
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Space pressed");

            onSpaceButtonPressed?.Invoke(BattleController.Instance.getCurrentEntity().transform);
        }
        onMouseHover?.Invoke(mousePosition);
    }
}
