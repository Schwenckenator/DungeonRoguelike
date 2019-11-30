using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour
{
    public float panSpeed = 1f;
    public float boost = 2f;
    public float zoomSpeed = 1.5f;

    public float minZoomLevel = 1f;

    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = GetInputVector();
        float boostSpeed = Input.GetKey(KeyCode.LeftShift) ? boost : 1;
        //Camera size makes it relative to window size
        transform.Translate(move * panSpeed * boostSpeed * camera.orthographicSize * Time.deltaTime);

        if(Input.GetAxis("Mouse ScrollWheel") < 0) {
            
            camera.orthographicSize += zoomSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            float newSize = camera.orthographicSize - zoomSpeed;
            if(newSize < minZoomLevel) {
                newSize = minZoomLevel;
            }
            camera.orthographicSize = newSize;
        }
    }

    Vector2 GetInputVector() {
        float X = Input.GetAxis("Horizontal");
        float Y = Input.GetAxis("Vertical");

        return new Vector2(X, Y);
    }
}
