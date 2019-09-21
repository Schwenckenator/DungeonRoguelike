using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetingRing : MonoBehaviour
{
    public Entity currentEntity;
    public static TargetingRing Instance { get; private set; }

    private Camera mainCamera;
    private new SpriteRenderer renderer;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        renderer = GetComponent<SpriteRenderer>();
        SetEnabled(false);
    }

    public void SetEnabled(bool value) {
        renderer.enabled = false;
        gameObject.SetActive(value);
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            renderer.enabled = false;
            return;
        } else {
            renderer.enabled = true;
        }
        Vector3 worldPoint3d = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPoint = new Vector2(worldPoint3d.x, worldPoint3d.y);

        Vector2 movePoint = worldPoint;

        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        foreach(var hit in hits) {

            //Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Entity")) {
                movePoint = hit.collider.transform.position;
            }
        }

        transform.position = movePoint;
    }
}
