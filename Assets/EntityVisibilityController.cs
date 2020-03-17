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

    /// <summary>
    /// Use this so to set living enitities to have the priority on the ui visibility
    /// </summary>
    public void DowngradeVisibilityLayer()
    {

        UICanvas.sortingOrder = -1;
        spriteRenderer.sortingOrder = -1;

    }

    /// <summary>
    /// Use this so to restore original ui visibility state
    /// </summary>
    public void RestoreVisibilityLayer()
    {
        //TODO maybe should keep the original value in case we want to use different sorting layers in the future.
        //Maybe just overengineering for now
        UICanvas.sortingOrder = 0;
        spriteRenderer.sortingOrder = 0;
         
    }

    public void SetDeadSprite(Character character)
    {

        spriteRenderer.sprite = character.deadSprite;

    }


}
