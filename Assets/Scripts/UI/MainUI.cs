using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{

    public void Attack() {

    }

    public void Heal() {

    }

    public void EndTurn() {
        BattleController.Instance.NextTurn();
    }
}
