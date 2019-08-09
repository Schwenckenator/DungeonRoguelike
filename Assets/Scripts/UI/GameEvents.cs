using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onStartPlayerTurn;

    public void  StartPlayerTurn(int entityID)
    {
        if(onStartPlayerTurn != null)
        {
            onStartPlayerTurn(entityID);
        }
    }

    public event Action<int> onFinishPlayerTurn;

    public void FinishPlayerTurn(int entityID)
    {
        if (onFinishPlayerTurn != null)
        {
            onFinishPlayerTurn(entityID);
        }
    }
}
