using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class has methods which hook the code to the UI
/// It should not do any functions on its own.
/// </summary>
public class MainUI : MonoBehaviour
{

    public void Interact(string type) {
        //For Current entity, Change state to targeting.
        BattleController.Instance.currentEntity.State = EntityState.targeting;

        if (type == "attack") { //TODO: Maybe a dictionary / way better way to do it would be good here
            BattleController.Instance.currentEntity.Interaction.myAbility = new BasicAttack(TargetType.enemy, 15, 30);
        }else if(type == "heal") {
            BattleController.Instance.currentEntity.Interaction.myAbility = new BasicAttack(TargetType.enemy, -15, -30);
        }
        
    }


    public void EndTurn() {
        BattleController.Instance.NextTurn();
    }
}
