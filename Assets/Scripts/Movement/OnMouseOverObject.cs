using UnityEngine;

public class OnMouseOverObject : MonoBehaviour
{
    bool highlighted = true;
    public GameObject highlightSprite;
    public Color unhighlightedColor;
    public Color highlightedColor;
    SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = highlightSprite.GetComponent<SpriteRenderer>();
        renderer.material.color = unhighlightedColor;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        //  Debug.Log("Mouse is over GameObject.");
        if (!highlighted)
        {

            renderer.material.color = highlightedColor;
        }
    }

    void OnMouseExit()
    {
        highlighted = false;
        renderer.material.color = unhighlightedColor;

        //The mouse is no longer hovering over the GameObject so output this message each frame
        //  Debug.Log("Mouse is no longer on GameObject.");
    }
}
