using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class HideOverTime : MonoBehaviour
//{
//    public int activeTurnsLifetime;
//    private int remainingActiveTurns;
//    public float hiddenAlpha = 0.5f;
//    private Entity parentEntity;


//    public void ActivateHideOverTime(Entity entity)
//    {
//        //Subscribe to player turn to decrement hide lifetime
//        GameEvents.current.onStartPlayerTurn += DecrementEffectLifetime;
//        parentEntity = entity;
//        EnterHide(parentEntity);
//        remainingActiveTurns = activeTurnsLifetime;

//    }

//    private void OnDestroy()
//    {
 
//        Destroy(gameObject);
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    private void EnterHide(Entity me)
//    {
//        me.EntityVisibilityController.SetEntityAlpha(hiddenAlpha);
//        //Use this to remove as a valid target for enemies
//        me.Stats.isHidden = true;
//     }
//    public void ExitHide()
//    {
//        GameEvents.current.onStartPlayerTurn -= DecrementEffectLifetime;
//        parentEntity.EntityVisibilityController.SetEntityAlpha(1);
//        parentEntity.Stats.isHidden = false;

//    }

//    private void DecrementEffectLifetime(int entityID)
//    {
//        // Check only get responding to relevant entity
//        if (entityID == parentEntity.gameObject.GetInstanceID())
//        {
//            remainingActiveTurns -= 1;
//            if (remainingActiveTurns <= 0)
//            {
//                ExitHide();
//                Destroy(this);
//            }
//        }
//    }
//}
