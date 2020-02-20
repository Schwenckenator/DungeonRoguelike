using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttribute
{
    public string Name { get; }
    public int Value { get; set; }

    public EntityAttribute() {
        Name = "";
        Value = 0;
    }

    public EntityAttribute(string name, int value) {
        Name = name;
        Value = value;
    }
}
