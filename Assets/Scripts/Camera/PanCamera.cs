using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour
{
    public float panSpeed = 1f;
    public float boost = 2f;
    public float zoomSpeed = 1.5f;

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
        //Camera size makes it relative to window size
        transform.Translate(move * panSpeed * camera.orthographicSize * Time.deltaTime);

        if(Input.GetAxis("Mouse ScrollWheel") < 0) {
            camera.orthographicSize += zoomSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            camera.orthographicSize -= zoomSpeed;
        }
    }

    Vector2 GetInputVector() {
        float X = Input.GetAxis("Horizontal");
        float Y = Input.GetAxis("Vertical");

        return new Vector2(X, Y);
    }
}
