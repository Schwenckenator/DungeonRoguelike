using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Operation { add, mult}

[System.Serializable]
public struct StatModifier
{
    public string id;
    public StatType attribute;
    public Operation operation;
    public float value;

    public StatModifier(string id, StatType attribute, Operation operation, float value) {
        this.id = id;
        this.attribute = attribute;
        this.operation = operation;
        this.value = value;
    }

    /// <summary>
    /// Returns the number to add
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public int Operate(int input) {
        if(operation == Operation.add) {
            return Mathf.RoundToInt(value);
        }
        if(operation == Operation.mult) {
            return Mathf.RoundToInt(value * input);
        }
        return 0;
    }

}
