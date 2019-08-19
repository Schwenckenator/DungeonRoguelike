using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusOnUnit : MonoBehaviour
{
    public static FocusOnUnit Instance { get; private set; }

    public float smoothTime = 0.3f;

    private Vector2 velocity = Vector2.zero;

    PanCamera panCamera;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        panCamera = GetComponent<PanCamera>();
        Instance = this;
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = Vector2.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
        newPosition += new Vector3(0, 0, -10);

        transform.position = newPosition;

        if(((Vector2)transform.position - (Vector2)target.position).sqrMagnitude < 0.1f) { // If the square mag is small
            SetEnabled(false);
        }
    }

    public void MoveCameraToUnit(Transform unit) {
        SetEnabled(true);
        target = unit;
    }

    private void SetEnabled(bool enabled) {
        this.enabled = enabled;
        panCamera.enabled = !enabled;
    }
}
