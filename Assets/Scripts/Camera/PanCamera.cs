using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour
{
    public float panSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = GetInputVector();
        transform.Translate(move * panSpeed * Time.deltaTime);
    }

    Vector2 GetInputVector() {
        float X = Input.GetAxis("Horizontal");
        float Y = Input.GetAxis("Vertical");

        return new Vector2(X, Y);
    }
}
