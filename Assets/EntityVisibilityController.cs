using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisibilityController : MonoBehaviour
{

    //Reference for modifying layer options
    public Canvas UICanvas;

    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void DowngradeVisibilityLayer()
    {

        UICanvas.sortingLayerID = -1;
        spriteRenderer.sortingLayerID = -1;

    }
    public void RestoreVisibilityLayer()
    {

        UICanvas.sortingLayerID = 0;
        spriteRenderer.sortingLayerID = 0;

    }

 
    //// Update is called once per frame
    //void Update()
    //{

    //}
}
