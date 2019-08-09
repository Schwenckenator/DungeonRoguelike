using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class has methods which hook the code to the UI
/// It should not do any functions on its own.
/// </summary>
public class MainUI : MonoBehaviour
{

    public void Attack() {
        BattleController.Instance.currentEntity;
    }

    private void DoDamage() {

    }

    public void Heal() {

    }

    public void EndTurn() {
        BattleController.Instance.NextTurn();
    }
}
