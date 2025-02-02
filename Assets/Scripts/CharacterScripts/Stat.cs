﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Stat
{
    [SerializeField] private string name;
    [SerializeField] private StatType type;
    [SerializeField] private int valueNow;
    [SerializeField] private int maxValue;



    //private Stat parent;
    //private string formula;
 
    public string Name {
        get{ return name;}
    }
    public StatType Type {
        get { return type; }
    }

    /// <summary>
    /// Sets value within bounds of 0 and max.
    /// </summary>
    public int ValueNow
    {
        get { return valueNow; }
        set
        {
            valueNow = value;
            if (valueNow > maxValue) valueNow = maxValue;
            if (valueNow < 0) valueNow = 0;
        }
    }
    
    /// <summary>
    /// Sets both value and max value.
    /// </summary>
    public int Value
    {
        get
        {
            return valueNow;
        }
        set
        {
            valueNow = value;
            maxValue = value;
        }
    }

    /// <summary>
    /// Sets the maximum value, which will constrain ValueNow.
    /// </summary>
    public int Max
    {
        get { return maxValue; }
        //TODO: make smart adjustment that remembers difference between valueNow and maxValue
        set { maxValue = value; }
    }

    public Stat(StatType type, int value) {
        this.type = type;
        this.name = type.ToString();
        this.maxValue = value;
        this.valueNow = value;
        //this.parent = parent;
    }
}
