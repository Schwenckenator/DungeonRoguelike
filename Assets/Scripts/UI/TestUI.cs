using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUI : MonoBehaviour
{
    public void StartBattle() {
        BattleController.Instance.StartBattle();
    }
}
