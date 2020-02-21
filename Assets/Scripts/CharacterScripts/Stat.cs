using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stat
{
    [SerializeField] private string name;
    [SerializeField] private int value;
    [SerializeField] private int max;
 

    public Stat(string name, int value) {
        this.name = name;
        this.max = value;
        this.value = value;
    }
}
