using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RangeInt{
    public int Min;
    public int Max;

    public RangeInt(int min, int max) {
        if(min > max) {
            Debug.LogError("Minimum is greater than Maximum! Wrong way around!");
            throw new System.Exception();
        }

        Min = min;
        Max = max;
    }

    public int GetRandom() {
        return Random.Range(Min, Max);
    }
    
}
[System.Serializable]
public class RangeFloat {
    public float Min;
    public float Max;

    public RangeFloat(float min, float max) {

        if (min > max) {
            Debug.LogError("Minimum is greater than Maximum! Wrong way around!");
            throw new System.Exception();
        }

        Min = min;
        Max = max;
    }

    public float GetRandom() {
        return Random.Range(Min, Max);
    }
}
