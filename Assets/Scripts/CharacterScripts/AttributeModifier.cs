using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Operation { add, mult}

[System.Serializable]
public struct AttributeModifier
{
    public Attribute attribute;
    public Operation operation;
    public float value;

    public AttributeModifier(Attribute attribute, Operation operation, float value) {
        this.attribute = attribute;
        this.operation = operation;
        this.value = value;
    }

}
