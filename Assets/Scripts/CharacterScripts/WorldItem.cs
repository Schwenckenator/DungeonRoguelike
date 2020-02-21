using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public Item myItem;
    // Start is called before the first frame update

    public void Initialise(Item newItem) {
        myItem = newItem;
        GetComponent<SpriteRenderer>().sprite = newItem.sprite;
    }

    public void PickUp(Entity entity) {
        entity.Inventory.AddItem(myItem);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Entity")){
            collision.GetComponent<Entity>().Inventory.OnItemCollisionEnter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Entity")) {
            collision.GetComponent<Entity>().Inventory.OnItemCollisionExit(this);
        }
    }
}
