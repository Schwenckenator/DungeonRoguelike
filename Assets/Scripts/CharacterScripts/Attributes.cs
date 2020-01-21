using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that holds attributes and modifiers for characters. 
/// Safer than a Dictionary I hope!
/// </summary>
[System.Serializable]
public class Attributes
{
    [SerializeField]
    private int vitalityBase;
    [SerializeField]
    private int mightBase;
    [SerializeField]
    private int graceBase;

    private int healthMax;

    //Put derived value formulas in here?
    public int VitalityBase { get
        {
            return vitalityBase;
        }
        set
        {
            vitalityBase = value;
        }
    }
    public int MightBase {
        get
        {
            return mightBase;
        }
        set
        {
            mightBase = value;
        }
    }
    public int GraceBase {
        get
        {
            return graceBase;
        }
        set
        {
            graceBase = value;
        }
    }

    public int VitalityNow { get; protected set; }
    public int MightNow { get; protected set; }
    public int GraceNow { get; protected set; }

    public int HealthMax {
        get
        {
            //Debug.Log($"Checking health Max. Vitality {VitalityNow")
            if(healthMax == 0) {
                healthMax = VitalityBase * 10;
            }
            return healthMax;
        }
        protected set
        {
            healthMax = value;
        }
    }
    public int HealthNow { get; set; }

}
