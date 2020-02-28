using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour
{
    public float panSpeed = 1f;
    public float boost = 2f;
    public float zoomSpeed = 1.5f;

    public float minZoomLevel = 1f;
    public float maxZoomLevel = 60f;

    public bool startAtMaxZoom = false;
    //Added this because its really been bugging me
    public bool panOnZoom = false;


    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

        if (startAtMaxZoom) {
            camera.orthographicSize = maxZoomLevel;
        }
    }

    // Update is called once per frame
    void Update() {

        Translate();
        //AdjustZoom();

        // Scroll forward
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
        }

        // Scoll back
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            ZoomOrthoCamera(Camera.main.ScreenToWorldPoint(Input.mousePosition), -1);
        }

    }

    //private void AdjustZoom() {
    //    if (Input.GetAxis("Mouse ScrollWheel") < 0) {
    //        float newSize = camera.orthographicSize + zoomSpeed;
    //        if (newSize > maxZoomLevel) {
    //            newSize = maxZoomLevel;
    //        }
    //        camera.orthographicSize = newSize;
    //    }
    //    if (Input.GetAxis("Mouse ScrollWheel") > 0) {
    //        float newSize = camera.orthographicSize - zoomSpeed;
    //        if (newSize < minZoomLevel) {
    //            newSize = minZoomLevel;
    //        }
    //        camera.orthographicSize = newSize;
    //    }
    //}


    // Ortographic camera zoom towards a point (in world coordinates). Negative amount zooms in, positive zooms out
    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        // Calculate how much we will have to move towards the zoomTowards position
        float multiplier = (1.0f / this.camera.orthographicSize * amount);

        // Move camera
        if(panOnZoom && this.camera.orthographicSize>minZoomLevel && this.camera.orthographicSize< maxZoomLevel)
        {
            transform.position += (zoomTowards - transform.position) * multiplier;
        }

        // Zoom camera
        this.camera.orthographicSize -= amount;

        // Limit zoom
        this.camera.orthographicSize = Mathf.Clamp(this.camera.orthographicSize, minZoomLevel, maxZoomLevel);
    }


    private void Translate() {
        Vector2 move = GetInputVector();
        float boostSpeed = Input.GetKey(KeyCode.LeftShift) ? boost : 1;
        //Camera size makes it relative to window size
        transform.Translate(move * panSpeed * boostSpeed * camera.orthographicSize * Time.deltaTime);
    }

    Vector2 GetInputVector() {
        float X = Input.GetAxis("Horizontal");
        float Y = Input.GetAxis("Vertical");

        return new Vector2(X, Y);
    }
}
