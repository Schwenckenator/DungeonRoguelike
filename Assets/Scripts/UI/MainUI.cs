using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class has methods which hook the code to the UI
/// It should not do any functions on its own.
/// </summary>
public class MainUI : MonoBehaviour
{

    public void Interact(int index) {
        //Obtain reference for ease of use
        Entity currentEntity = BattleController.Instance.currentEntity;

        //For Current entity, Change state to targeting.
        currentEntity.State = EntityState.targeting;
        currentEntity.Interaction.SetCurrentAbility(index);
        
    }


    public void EndTurn() {
        BattleController.Instance.NextTurn();
    }
}
