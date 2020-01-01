using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickCollisionDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
            DetectCollision();
        }
    }

    void DetectCollision() {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.OverlapCircleAll(point, 0.45f);

        Debug.Log($"Mouse Position: {Input.mousePosition}, World Position: {point} hits are...");
        foreach(var hit in hits) {
            Debug.Log($"{hit} is hit and on layer {LayerMask.LayerToName(hit.gameObject.layer)}.");
        }

        var layerHits = Physics2D.OverlapCircleAll(point, 0.45f, LayerMask.GetMask("Obstacle"));
        Debug.Log("Layer masked hits are...");
        foreach (var hit in layerHits) {
            Debug.Log($"{hit} is hit and on layer {LayerMask.LayerToName(hit.gameObject.layer)}.");
        }
    }
}
